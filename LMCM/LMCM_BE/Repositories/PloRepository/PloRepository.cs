using LMCM_BE.DbContext;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.PloRepository;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.PloRepository
{
    public class PloRepository : IPloRepository
    {
        private readonly LMCM_DBContext _dbContext;

        public PloRepository(LMCM_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<Plo>> GetPlosAsync(Guid curriculumId, string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.Plos
                .Where(p => p.CurriculumId == curriculumId && p.Status != "Inactive")
                .Include(p => p.PloSubjects)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                query = query.Where(p => p.PloName.ToLower().Contains(search));
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return new PagedResult<Plo>
            {
                Items = items,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }
        public async Task<List<Plo>> GetAllPlosAsync(Guid curriculumId)
        {
            return await _dbContext.Plos
                .Where(p => p.CurriculumId == curriculumId && p.Status != "Deleted")
                .Include(p => p.PloSubjects)
                .ToListAsync();
        }

        public async Task<bool> AddPlosAsync(List<Plo> plos)
        {
            if (plos == null || plos.Count == 0)
                throw new ArgumentNullException(nameof(plos));

            try
            {
                await _dbContext.Plos.AddRangeAsync(plos);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeletePlosAsync(List<Guid> ploIds)
        {
            var plos = await _dbContext.Plos.Where(p => ploIds.Contains(p.PloId) && p.Status != "Inactive").ToListAsync();
            if (!plos.Any()) return false;

            foreach (var plo in plos)
            {
                plo.Status = "Inactive";
                plo.UpdatedAt = DateTime.UtcNow;
            }

            _dbContext.Plos.UpdateRange(plos);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
