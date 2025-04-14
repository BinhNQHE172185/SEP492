using AutoMapper;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SubjectDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.CurriculumsSubjectRepository;
using LMCM_BE.Repositories.PloSubjectRepository;
using LMCM_BE.Repositories.SubjectRepository.SubjectRepository;
using LMCM_BE.Repositories.SyllabusRepository;
using LMCM_BE.Shared.Constant;
using LMCM_BE.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace LMCM_BE.Services.SubjectService
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPloSubjectRepository _ploSubjectRepository;
        private readonly ICurriculumsSubjectRepository _curriculumSubjectRepository;
        private readonly ISyllabusRepository _syllabusSubjectRepository;
        public SubjectService(ISubjectRepository subjectRepository, IMapper mapper, IUnitOfWork unitOfWork, IPloSubjectRepository ploSubjectRepository, ICurriculumsSubjectRepository curriculumSubjectRepository, ISyllabusRepository syllabusRepository)
        {
            _subjectRepository = subjectRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _curriculumSubjectRepository = curriculumSubjectRepository;
            _ploSubjectRepository = ploSubjectRepository;
            _syllabusSubjectRepository = syllabusRepository;
        }

        public async Task<int> getSubjectCountAsync()
        {
            return await _subjectRepository.CountSubjectByStatusAsync(GenericStatus.Active);
        }

        public async Task<PagedResult<SubjectViewDto>> GetSubjectsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var (data, totalCount) = await _subjectRepository.GetSubjectsAsync(searchKey, pageIndex, pageSize);

            var dataDtos = _mapper.Map<List<SubjectViewDto>>(data);

            return new PagedResult<SubjectViewDto>
            {
                Items = dataDtos,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };

        }

        public async Task<bool> ImportSubjectsAsync(ExcelWorksheet worksheet)
        {
            // Validate Headers
            string[] expectedHeaders = {
                    "SubjectCode", "SubjectName", "English SubjectName", "PreviousCode",
                    "Is Constructivist Subject", "Method", "Duration", "Reality"
                };

            for (int col = 1; col <= expectedHeaders.Length; col++)
            {
                if (worksheet.Cells[1, col].Text.Trim() != expectedHeaders[col - 1])
                {
                    throw new InvalidDataException("Định dạng Excel không hợp lệ. Vui lòng sử dụng mẫu đúng.");
                }
            }

            // Read and Process Data
            int rowCount = worksheet.Dimension.Rows;

            List<SubjectInsertDto> subjectDtos = new List<SubjectInsertDto>();
            HashSet<string> subjectCodes = new HashSet<string>();

            for (int row = 2; row <= rowCount; row++)
            {
                string subjectCode = worksheet.Cells[row, 1].Text;

                if (subjectCodes.Contains(subjectCode))
                {
                    throw new InvalidDataException($"Tìm thấy mã môn học trùng lặp trong tệp Excel: {subjectCode} tại hàng {row}");
                }
                subjectCodes.Add(subjectCode);

                var subject = new SubjectInsertDto
                {
                    SubjectId = Guid.NewGuid(),
                    SubjectCode = subjectCode,
                    SubjectName = worksheet.Cells[row, 2].Text,
                    SubjectNameEnglish = worksheet.Cells[row, 3].Text,
                    PreviousSubjectCode = worksheet.Cells[row, 4].Text,
                    IsConstructivist = worksheet.Cells[row, 5].Text.ToLower() == "true",
                    Method = worksheet.Cells[row, 6].Text,
                    Duration = int.TryParse(worksheet.Cells[row, 7].Text, out int duration) ? duration : 0,
                    Reality = int.TryParse(worksheet.Cells[row, 8].Text, out int reality) ? reality : 0
                };

                subjectDtos.Add(subject);
            }

            if (subjectDtos == null || subjectDtos.Count == 0)
                throw new InvalidDataException("Danh sách môn học không bị trống");

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var subjects = _mapper.Map<List<Subject>>(subjectDtos);

                // Get existing subjects from DB (as a dictionary for fast lookup)
                var existingSubjects = await _subjectRepository.GetSubjectsDictonaryAsync();

                var subjectCodesToKeep = subjects.Select(s => s.SubjectCode).ToHashSet();
                var newSubjects = new List<Subject>();
                var updatedSubjects = new List<Subject>();

                foreach (var subject in subjects)
                {
                    if (existingSubjects.TryGetValue(subject.SubjectCode, out var existingSubject))
                    {
                        if (await UpdateSubjectIfChangedAsync(existingSubject, subject))
                        {
                            existingSubject.UpdatedAt = DateTime.UtcNow;
                            updatedSubjects.Add(existingSubject);
                        }
                    }
                    else
                    {
                        var newSubject = subject;
                        newSubject.SubjectId = Guid.NewGuid();
                        newSubject.Status = GenericStatus.Active;
                        newSubject.CreatedAt = DateTime.UtcNow;
                        newSubject.UpdatedAt = DateTime.UtcNow;
                        newSubjects.Add(newSubject);
                    }
                }

                // Identify subjects to mark as inactive
                var subjectsToDeactivate = existingSubjects.Values.Where(s => !subjectCodesToKeep.Contains(s.SubjectCode)).ToList();
                foreach (var subject in subjectsToDeactivate)
                {
                    subject.Status = GenericStatus.Inactive;
                    subject.UpdatedAt = DateTime.UtcNow;
                    updatedSubjects.Add(subject);
                }

                // Apply updates and inserts
                if (updatedSubjects.Any())
                    await _subjectRepository.UpdateSubjectsAsync(updatedSubjects);

                if (newSubjects.Any())
                    await _subjectRepository.AddSubjectsAsync(newSubjects);

                await _unitOfWork.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
        private async Task<bool> UpdateSubjectIfChangedAsync(Subject existingSubject, Subject newSubject)
        {
            bool isUpdated = false;

            if (existingSubject.SubjectName != newSubject.SubjectName)
            {
                existingSubject.SubjectName = newSubject.SubjectName;
                isUpdated = true;
            }
            if (existingSubject.SubjectNameEnglish != newSubject.SubjectNameEnglish)
            {
                existingSubject.SubjectNameEnglish = newSubject.SubjectNameEnglish;
                isUpdated = true;
            }
            if (existingSubject.IsConstructivist != newSubject.IsConstructivist)
            {
                existingSubject.IsConstructivist = newSubject.IsConstructivist;
                isUpdated = true;
            }
            if (existingSubject.Method != newSubject.Method)
            {
                existingSubject.Method = newSubject.Method;
                isUpdated = true;
            }
            if (existingSubject.Duration != newSubject.Duration)
            {
                existingSubject.Duration = newSubject.Duration;
                isUpdated = true;
            }
            if (existingSubject.Reality != newSubject.Reality)
            {
                existingSubject.Reality = newSubject.Reality;
                isUpdated = true;
            }
            if (existingSubject.Status == GenericStatus.Inactive)
            {
                existingSubject.Status = GenericStatus.Active;
                isUpdated = true;
            }

            return await Task.FromResult(isUpdated);
        }
        public async Task<Subject> GetSubjectByCodeAsync(string subjectCode)
        {
            if (string.IsNullOrEmpty(subjectCode))
                throw new ArgumentNullException("Subject code bị trống.", nameof(subjectCode));
            return await _subjectRepository.GetSubjectByCodeAsync(subjectCode);
        }
        public async Task<List<Subject>> GetActiveSubjectsByCodesAsync(List<string> subjectCodes)
        {
            return await _subjectRepository.GetActiveSubjectsByCodesAsync(subjectCodes);
        }
        public async Task<bool> SoftDeleteSubjectAsync(Guid subjectId)
        {
            Subject subject = await _subjectRepository.GetSubjectByIdAsync(subjectId);
            if (subject == null)
                throw new KeyNotFoundException(nameof(subject));
            if (await _curriculumSubjectRepository.HasActiveCurriculumSubjectsBySubjectIdAsync(subjectId) ||
                await _ploSubjectRepository.HasActivePloSubjectBySubjectIdAsync(subjectId) ||
                await _syllabusSubjectRepository.GetActiveSyllabusBySubjectIdAsync(subjectId) != null)
            {
                throw new InvalidOperationException("Không thể xóa môn học khi có thực thể liên quan đang hoạt động.");
            }
            subject.Status = GenericStatus.Inactive;
            subject.UpdatedAt = DateTime.UtcNow;
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _subjectRepository.UpdateSubjectAsync(subject);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
    }
}
