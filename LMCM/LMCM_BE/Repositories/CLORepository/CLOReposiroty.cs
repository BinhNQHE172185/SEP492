using LMCM_BE.DbContext;
using LMCM_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.CLORepository
{
    public class CLOReposiroty : ICLORepository
    {
        private readonly LMCM_DBContext _dbContext;
        public CLOReposiroty(LMCM_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> UpdateCLOsAsync(List<Clo> clos)
        {
            _dbContext.Clos.UpdateRange(clos);

            return true;
        }

        public async Task<bool> AddCLOsAsync(List<Clo> cLOs)
        {
            await _dbContext.Clos.AddRangeAsync(cLOs);

            return true;
        }

        public async Task<List<Clo>> GetCLOsBySyllabusASync(Guid syllabusId)
        {
            var clos = await _dbContext.Clos
                .Where(c => c.SyllabusId == syllabusId)
                .ToListAsync();
            return clos;
        }
    }
}
