using AutoMapper;
using LMCM_BE.DbContext;
using LMCM_BE.Models;
using LMCM_BE.Models.Constant;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.PloRepository
{
    public class PloRepository : IPloRepository
    {
        private readonly LMCM_DBContext _dbContext;


        public PloRepository(LMCM_DBContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Plo>> GetPloDetailsByCurriculumIdAsync(Guid curriculumId)
        {
            return await _dbContext.Plos
                .Include(p => p.PloSubjects)
                    .ThenInclude(ps => ps.Subject)
                .Where(p => p.CurriculumId == curriculumId && p.Status == GenericStatus.Active)
                .OrderBy(p=>p.PloName)
                .ToListAsync();
        }
        public async Task<List<Plo>> GetPloByCurriculumIdAsync(Guid curriculumId)
        {
            return await _dbContext.Plos
                .Where(p => p.CurriculumId == curriculumId && p.Status == GenericStatus.Active)
                .ToListAsync();
        }
        public async Task<bool> UpdateRangeAsync(List<Plo> entities)
        {
            _dbContext.Plos.UpdateRange(entities);
            return true;
        }
        public async Task<bool> HasActivePloAsync(Guid curriculumId)
        {
            return await _dbContext.Plos
                .AnyAsync(p => p.CurriculumId == curriculumId && p.Status == GenericStatus.Active);
        }

    }
}
