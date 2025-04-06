using AutoMapper;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.GradingStructureDtos;
using LMCM_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.GradingStructureRepository
{
    public class GradingStructureRepository : IGradingStructureRepository
    {
        private readonly LMCM_DBContext _dbContext;
        private readonly IMapper _mapper;
        public GradingStructureRepository(LMCM_DBContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<bool> DeleteGradingStructuresBySyllabusAsync(Guid syllabusId)
        {
            if (syllabusId == Guid.Empty)
                throw new ArgumentException("Syllabus ID cannot be empty.", nameof(syllabusId));

            try
            {
                var gradingStructures = await _dbContext.GradingStructures
                    .Where(s => s.SyllabusId == syllabusId)
                    .ToListAsync();

                if (!gradingStructures.Any())
                    return false; // No grading structures found for the given syllabus

                foreach (var gradingStructure in gradingStructures)
                {
                    gradingStructure.Status = "Inactive";
                    gradingStructure.UpdatedAt = DateTime.UtcNow;
                }

                _dbContext.GradingStructures.UpdateRange(gradingStructures);

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

        public async Task<bool> ImportGradingStructuresAsync(List<GradingStructureInsertDto> gradingStructures, Guid syllabusId)
        {
            if (gradingStructures == null || !gradingStructures.Any())
                throw new ArgumentNullException(nameof(gradingStructures));

            var newGradingStructures = _mapper.Map<List<GradingStructure>>(gradingStructures);

            foreach (var structure in newGradingStructures)
            {
                structure.SyllabusId = syllabusId;
                structure.StructureId = Guid.NewGuid();
                structure.Status = "Active";
                structure.CreatedAt = DateTime.UtcNow;
                structure.UpdatedAt = DateTime.UtcNow;
            }

            await _dbContext.GradingStructures.AddRangeAsync(newGradingStructures);

            return true;
        }
    }
}
