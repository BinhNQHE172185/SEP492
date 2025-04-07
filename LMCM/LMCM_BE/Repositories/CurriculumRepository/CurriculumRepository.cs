using LMCM_BE.DbContext;
using LMCM_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.CurriculumRepository
{
    public class CurriculumRepository : ICurriculumRepository
    {
        private readonly LMCM_DBContext _dbContext;

        public CurriculumRepository(
            LMCM_DBContext dbContext
            )
        {
            _dbContext = dbContext;
        }
        public async Task<(List<Curriculum>, int totalCount)> GetCurriculumsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.Curriculums.Include(c => c.CurriculumsSubjects).Where(c => c.Status == "Active").AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                query = query.Where(c => c.CurriculumCode.ToLower().Contains(search) ||
                                         c.CurriculumName.ToLower().Contains(search));
            }

            int totalCount = await query.CountAsync();

            var curriculums = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();
            return (curriculums, totalCount);
        }
        public async Task<bool> UpdateCurriculumAsync(Curriculum curriculum)
        {
            _dbContext.Curriculums.Update(curriculum);

            return true;
        }
         public async Task<Curriculum?> GetCurriculumDetailAsync(Guid curriculumId)
        {
            return await _dbContext.Curriculums
                .Include(c => c.CurriculumsSubjects)
                    .ThenInclude(cs => cs.Subject)
                .Where(c => c.CurriculumId == curriculumId && c.Status == "Active")
                .FirstOrDefaultAsync();
        }
        public async Task<Curriculum?> GetActiveCurriculumByCodeAsync(string curriculumCode)
        {
            return await _dbContext.Curriculums
                .FirstOrDefaultAsync(c => c.CurriculumCode == curriculumCode && c.Status == "Active");
        }
        public async Task<Curriculum?> GetActiveCurriculumByIdAsync(Guid curriculumId)
        {
            return await _dbContext.Curriculums
                .FirstOrDefaultAsync(c => c.CurriculumId == curriculumId && c.Status == "Active");
        }
        public async Task<bool> ImportCurriculumAsync(Curriculum curriculum)
        {
            _dbContext.Curriculums.Add(curriculum);
            return true;
        }
    }
}
