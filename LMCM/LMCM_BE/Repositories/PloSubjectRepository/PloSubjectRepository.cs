using LMCM_BE.DbContext;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.PloSubjectRepository
{
    public class PloSubjectRepository : IPloSubjectRepository
    {
        private readonly LMCM_DBContext _dbContext;

        public PloSubjectRepository(LMCM_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<PloSubject>> GetPloSubjectsAsync(Guid ploId, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.PloSubjects
                .Where(ps => ps.PloId == ploId && ps.Status != "Deleted")
                .Include(ps => ps.Subject)
                .AsQueryable();

            int totalCount = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return new PagedResult<PloSubject>
            {
                Items = items,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<List<PloSubject>> GetAllPloSubjectsAsync(Guid ploId)
        {
            return await _dbContext.PloSubjects
                .Where(ps => ps.PloId == ploId && ps.Status != "Deleted")
                .Include(ps => ps.Subject)
                .ToListAsync();
        }

        public async Task<bool> AddPloSubjectsAsync(List<PloSubject> ploSubjects)
        {
            if (ploSubjects == null || ploSubjects.Count == 0)
                throw new ArgumentNullException(nameof(ploSubjects));

            try
            {
                await _dbContext.PloSubjects.AddRangeAsync(ploSubjects);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeletePloSubjectsAsync(List<Guid> ploIds)
        {
            var ploSubjects = await _dbContext.PloSubjects
                .Where(ps => ploIds.Contains(ps.PloId) && ps.Status != "Deleted")
                .ToListAsync();

            if (!ploSubjects.Any()) return false;

            foreach (var ploSubject in ploSubjects)
            {
                ploSubject.Status = "Deleted";
                ploSubject.UpdatedAt = DateTime.UtcNow;
            }

            _dbContext.PloSubjects.UpdateRange(ploSubjects);
            await _dbContext.SaveChangesAsync();
            return true;
        }

    }
}
