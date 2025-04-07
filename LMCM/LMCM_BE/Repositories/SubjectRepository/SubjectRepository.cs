using LMCM_BE.DbContext;
using LMCM_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.SubjectRepository.SubjectRepository
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly LMCM_DBContext _dbContext;

        public SubjectRepository(LMCM_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(List<Subject>, int totalCount)> GetSubjectsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
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
            return (items, totalCount);

        }
        public async Task<List<Subject>> GetActiveSubjectsByCodesAsync(List<string> subjectCodes)
        {
            return await _dbContext.Subjects
                .Where(s => subjectCodes.Contains(s.SubjectCode) && s.Status == "Active")
                .ToListAsync();
        }

        public async Task<bool> ImportSubjectsAsync(List<Subject> subjects)
        {

            // Get existing subjects from DB (as a dictionary for fast lookup)
            var existingSubjects = await _dbContext.Subjects.ToDictionaryAsync(s => s.SubjectCode);

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

            return true;
        }
        public async Task<bool> UpdateSubjectIfChangedAsync(Subject existingSubject, Subject newSubject)
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
            if (existingSubject.Status != "Active")
            {
                existingSubject.Status = "Active";
                isUpdated = true;
            }

            return await Task.FromResult(isUpdated);
        }
        public async Task<Subject> GetSubjectByCodeAsync(string code)
        {

            var subject = await _dbContext.Subjects
                                          .FirstOrDefaultAsync(s => s.SubjectCode == code &&
                                                               (s.Status != null && s.Status.ToLower() == "active"));

            return subject;
        }
        public async Task<Subject> GetSubjectByIdAsync(Guid subjectId)
        {

            var subject = await _dbContext.Subjects
                                          .FirstOrDefaultAsync(s => s.SubjectId == subjectId &&
                                                               (s.Status != null && s.Status.ToLower() == "active"));

            return subject;
        }
        public async Task<bool> SoftDeleteSubjectAsync(Subject subject)
        {
            subject.Status = "Inactive";
            subject.UpdatedAt = DateTime.UtcNow;
            _dbContext.Subjects.Update(subject);

            return true;
        }

    }
}
