using AutoMapper;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SubjectDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.CurriculumsSubjectRepository;
using LMCM_BE.Repositories.PloSubjectRepository;
using LMCM_BE.Repositories.SyllabusRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LMCM_BE.Repositories.SubjectRepository.SubjectRepository
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly LMCM_DBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IPloSubjectRepository _ploSubjectRepository;
        private readonly ICurriculumsSubjectRepository _curriculumSubjectRepository;
        private readonly ISyllabusRepository _syllabusSubjectRepository;


        public SubjectRepository(LMCM_DBContext dbContext, IMapper mapper, IPloSubjectRepository ploSubjectRepository, ICurriculumsSubjectRepository curriculumSubjectRepository, ISyllabusRepository syllabusRepository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _curriculumSubjectRepository = curriculumSubjectRepository;
            _ploSubjectRepository = ploSubjectRepository;
            _syllabusSubjectRepository = syllabusRepository;
        }

        public async Task<PagedResult<SubjectViewDto>> GetSubjectsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.Subjects.AsQueryable();

            query = query.OrderBy(s => s.SubjectCode);

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                query = query.Where(s => s.SubjectCode.ToLower().Contains(search) ||
                                         s.SubjectName.ToLower().Contains(search));
            }

            query = query.Where(s => s.Status != "Inactive");

            int totalCount = await query.CountAsync();

            var items = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            var data = _mapper.Map<List<SubjectViewDto>>(items);

            return new PagedResult<SubjectViewDto>
            {
                Items = data,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }
        public async Task<List<Subject>> GetActiveSubjectsByCodesAsync(List<string> subjectCodes)
        {
            return await _dbContext.Subjects
                .Where(s => subjectCodes.Contains(s.SubjectCode) && s.Status == "Active")
                .ToListAsync();
        }
        public async Task<bool> InsertSubjectAsync(SubjectInsertDto subjectDto)
        {
            if (subjectDto == null) throw new ArgumentNullException(nameof(subjectDto));

            try
            {
                var subject = _mapper.Map<Subject>(subjectDto);
                subject.SubjectId = Guid.NewGuid();
                subject.CreatedAt = DateTime.UtcNow;
                subject.UpdatedAt = DateTime.UtcNow;

                await _dbContext.Subjects.AddAsync(subject);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ImportSubjectsAsync(List<SubjectInsertDto> subjects)
        {
            if (subjects == null || subjects.Count == 0)
                throw new ArgumentNullException(nameof(subjects));

            try
            {
                // Get existing subjects from DB (as a dictionary for fast lookup)
                var existingSubjects = await _dbContext.Subjects.ToDictionaryAsync(s => s.SubjectCode);

                var subjectCodesToKeep = subjects.Select(s => s.SubjectCode).ToHashSet();
                var newSubjects = new List<Subject>();
                var updatedSubjects = new List<Subject>();

                foreach (var subjectDto in subjects)
                {
                    if (existingSubjects.TryGetValue(subjectDto.SubjectCode, out var existingSubject))
                    {
                        if (await UpdateSubjectIfChangedAsync(existingSubject, subjectDto))
                        {
                            existingSubject.UpdatedAt = DateTime.UtcNow;
                            updatedSubjects.Add(existingSubject);
                        }
                    }
                    else
                    {
                        var newSubject = _mapper.Map<Subject>(subjectDto);
                        newSubject.SubjectId = Guid.NewGuid();
                        newSubject.Status = "Active";
                        newSubject.CreatedAt = DateTime.UtcNow;
                        newSubject.UpdatedAt = DateTime.UtcNow;
                        newSubjects.Add(newSubject);
                    }
                }

                // Identify subjects to mark as inactive
                var subjectsToDeactivate = existingSubjects.Values.Where(s => !subjectCodesToKeep.Contains(s.SubjectCode)).ToList();
                foreach (var subject in subjectsToDeactivate)
                {
                    subject.Status = "Inactive";
                    subject.UpdatedAt = DateTime.UtcNow;
                    updatedSubjects.Add(subject);
                }

                // Apply updates and inserts
                if (updatedSubjects.Any())
                    _dbContext.Subjects.UpdateRange(updatedSubjects);

                if (newSubjects.Any())
                    await _dbContext.Subjects.AddRangeAsync(newSubjects);

                if (updatedSubjects.Any() || newSubjects.Any())
                    await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> UpdateSubjectIfChangedAsync(Subject existingSubject, SubjectInsertDto subjectDto)
        {
            bool isUpdated = false;

            if (existingSubject.SubjectName != subjectDto.SubjectName)
            {
                existingSubject.SubjectName = subjectDto.SubjectName;
                isUpdated = true;
            }
            if (existingSubject.SubjectNameEnglish != subjectDto.SubjectNameEnglish)
            {
                existingSubject.SubjectNameEnglish = subjectDto.SubjectNameEnglish;
                isUpdated = true;
            }
            if (existingSubject.IsConstructivist != subjectDto.IsConstructivist)
            {
                existingSubject.IsConstructivist = subjectDto.IsConstructivist;
                isUpdated = true;
            }
            if (existingSubject.Method != subjectDto.Method)
            {
                existingSubject.Method = subjectDto.Method;
                isUpdated = true;
            }
            if (existingSubject.Duration != subjectDto.Duration)
            {
                existingSubject.Duration = subjectDto.Duration;
                isUpdated = true;
            }
            if (existingSubject.Reality != subjectDto.Reality)
            {
                existingSubject.Reality = subjectDto.Reality;
                isUpdated = true;
            }
            if (existingSubject.Status != "Active")
            {
                existingSubject.Status="Active";
                isUpdated = true;
            }

            return await Task.FromResult(isUpdated);
        }
        public async Task<Subject> GetSubjectByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentException("Subject code cannot be empty.", nameof(code));

            try
            {
                var subject = await _dbContext.Subjects
                                              .FirstOrDefaultAsync(s => s.SubjectCode == code &&
                                                                   (s.Status != null && s.Status.ToLower() == "active"));

                return subject;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<Subject> GetSubjectByIdAsync(Guid subjectId)
        {
            if (subjectId == null)
                throw new ArgumentException("Subject code cannot be empty.", nameof(subjectId));

            try
            {
                var subject = await _dbContext.Subjects
                                              .FirstOrDefaultAsync(s => s.SubjectId == subjectId &&
                                                                   (s.Status != null && s.Status.ToLower() == "active"));

                return subject;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<bool> SoftDeleteSubjectAsync(Guid subjectId)
        {
            // Step 1: Check if subject exists and is active
            var subject = await _dbContext.Subjects
                .FirstOrDefaultAsync(s => s.SubjectId == subjectId && s.Status == "Active");

            if (subject == null)
                return false; // Subject not found or already inactive

            // Step 2: Check if there are active related entities
            if (await _curriculumSubjectRepository.HasActiveCurriculumSubjectsBySubjectIdAsync(subjectId) ||
                await _ploSubjectRepository.HasActivePloSubjectBySubjectIdAsync(subjectId) || 
                (await _syllabusSubjectRepository.GetActiveSyllabusBySubjectIdAsync(subjectId) != null))
            {
                throw new InvalidOperationException("Không thể xóa môn học khi có thực thể liên quan đang hoạt động.");
            }

            try
            {
                using var transaction = await _dbContext.Database.BeginTransactionAsync();

                // Step 3: Soft delete the subject
                subject.Status = "Inactive";
                subject.UpdatedAt = DateTime.UtcNow;
                _dbContext.Subjects.Update(subject);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
