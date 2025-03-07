using LMCM_BE.DbContext;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.CurriculumsSubjectRepository
{
    public class CurriculumsSubjectRepository : ICurriculumsSubjectRepository
    {
        private readonly LMCM_DBContext _dbContext;

        public CurriculumsSubjectRepository(LMCM_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<CurriculumsSubject>> GetCurriculumsSubjectsAsync(Guid curriculumId, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.CurriculumsSubjects
                .Where(cs => cs.CurriculumId == curriculumId && cs.Status != "Deleted")
                .Include(cs => cs.Subject)
                .AsQueryable();

            int totalCount = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return new PagedResult<CurriculumsSubject>
            {
                Items = items,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<List<CurriculumsSubject>> GetAllCurriculumsSubjectsAsync(Guid curriculumId)
        {
            return await _dbContext.CurriculumsSubjects
                .Where(cs => cs.CurriculumId == curriculumId && cs.Status != "Deleted")
                .Include(cs => cs.Subject)
                .ToListAsync();
        }

        public async Task<bool> AddCurriculumsSubjectsAsync(List<CurriculumsSubject> curriculumsSubjects)
        {
            if (curriculumsSubjects == null || curriculumsSubjects.Count == 0)
                throw new ArgumentNullException(nameof(curriculumsSubjects));

            try
            {
                await _dbContext.CurriculumsSubjects.AddRangeAsync(curriculumsSubjects);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteCurriculumsSubjectAsync(Guid curriculumId)
        {
            var csList = await _dbContext.CurriculumsSubjects
                .Where(c => c.CurriculumId == curriculumId && c.Status != "Deleted")
                .ToListAsync();

            if (!csList.Any()) return false;

            foreach (var cs in csList)
            {
                cs.Status = "Deleted";
                cs.UpdatedAt = DateTime.UtcNow;
            }

            _dbContext.CurriculumsSubjects.UpdateRange(csList);
            await _dbContext.SaveChangesAsync();
            return true;
        }

    }
}
