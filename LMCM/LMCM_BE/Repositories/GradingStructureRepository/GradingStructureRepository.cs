using LMCM_BE.DbContext;
using LMCM_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.GradingStructureRepository
{
    public class GradingStructureRepository : IGradingStructureRepository
    {
        private readonly LMCM_DBContext _dbContext;
        public GradingStructureRepository(LMCM_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> UpdateGradingStructuresAsync(List<GradingStructure> gradingStructures)
        {
            _dbContext.GradingStructures.UpdateRange(gradingStructures);

            return true;
        }

        public async Task<bool> AddGradingStructuresAsync(List<GradingStructure> gradingStructures)
        {
            await _dbContext.GradingStructures.AddRangeAsync(gradingStructures);

            return true;
        }

        public async Task<List<GradingStructure>> GetGradingStructuresBySyllabusAsync(Guid syllabusId)
        {
            var gradingStructures = await _dbContext.GradingStructures
                .Where(s => s.SyllabusId == syllabusId)
                .ToListAsync();
            return gradingStructures;

        }
    }
}
