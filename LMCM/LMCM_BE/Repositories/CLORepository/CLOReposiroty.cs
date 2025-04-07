using AutoMapper;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.CLODtos;
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

        public async Task<bool> DeleteCLOBySyllabusAsync(Guid syllabusId)
        {
            var clos = await _dbContext.Clos
                .Where(c => c.SyllabusId == syllabusId)
                .ToListAsync();

            if (!clos.Any())
                return false; // No CLOs found for the given syllabus

            foreach (var clo in clos)
            {
                clo.Status = "Inactive";
                clo.UpdatedAt = DateTime.UtcNow;
            }

            _dbContext.Clos.UpdateRange(clos);

            return true;
        }

        public async Task<bool> ImportCLOsAsync(List<Clo> cLOs, Guid syllabusId)
        {
            if (cLOs == null || !cLOs.Any())
                throw new ArgumentNullException(nameof(cLOs));


            foreach (var clo in cLOs)
            {
                clo.SyllabusId = syllabusId;
                clo.CloId = Guid.NewGuid();
                clo.Status = "Active";
                clo.CreatedAt = DateTime.UtcNow;
                clo.UpdatedAt = DateTime.UtcNow;
            }

            await _dbContext.Clos.AddRangeAsync(cLOs);

            return true;
        }

    }
}
