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
        private readonly IMapper _mapper;
        public CLOReposiroty(LMCM_DBContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<bool> DeleteCLOBySyllabusAsync(Guid syllabusId)
        {
            if (syllabusId == Guid.Empty)
                throw new ArgumentException("Syllabus ID cannot be empty.", nameof(syllabusId));

            try
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
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine(dbEx.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> ImportCLOsAsync(List<CLOInsertDto> cLOs)
        {
            if (cLOs == null || !cLOs.Any())
                throw new ArgumentNullException(nameof(cLOs));

            var newCLOs = _mapper.Map<List<Clo>>(cLOs);

            foreach (var clo in newCLOs)
            {
                clo.CloId = Guid.NewGuid();
                clo.Status = "Active";
                clo.CreatedAt = DateTime.UtcNow;
                clo.UpdatedAt = DateTime.UtcNow;
            }

            await _dbContext.Clos.AddRangeAsync(newCLOs);

            return true;
        }

    }
}
