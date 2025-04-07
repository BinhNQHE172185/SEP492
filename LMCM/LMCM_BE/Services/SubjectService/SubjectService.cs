using AutoMapper;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SubjectDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.CurriculumsSubjectRepository;
using LMCM_BE.Repositories.PloSubjectRepository;
using LMCM_BE.Repositories.SubjectRepository.SubjectRepository;
using LMCM_BE.Repositories.SyllabusRepository;
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

                var isSuccess = await _subjectRepository.ImportSubjectsAsync(subjects);

                await _unitOfWork.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
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
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _subjectRepository.SoftDeleteSubjectAsync(subject);
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
