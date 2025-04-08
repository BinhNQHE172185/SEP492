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

        public async Task<bool> AddSubjectsAsync(List<Subject> subjects)
        {
            await _dbContext.Subjects.AddRangeAsync(subjects);

            return true;
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
        public async Task<bool> UpdateSubjectAsync(Subject subject)
        {
            _dbContext.Subjects.Update(subject);

            return true;
        }

        public async Task<Dictionary<string, Subject>> GetSubjectsDictonaryAsync()
        {
            return await _dbContext.Subjects.ToDictionaryAsync(s => s.SubjectCode);
        }

        public async Task<bool> UpdateSubjectsAsync(List<Subject> subjects)
        {
            _dbContext.Subjects.UpdateRange(subjects);

            return true;
        }
    }
}
