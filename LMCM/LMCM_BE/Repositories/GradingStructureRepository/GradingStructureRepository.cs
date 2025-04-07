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

        public async Task<bool> ImportGradingStructuresAsync(List<GradingStructure> gradingStructures, Guid syllabusId)
        {
            if (gradingStructures == null || !gradingStructures.Any())
                throw new ArgumentNullException(nameof(gradingStructures));

            foreach (var structure in gradingStructures)
            {
                structure.SyllabusId = syllabusId;
                structure.StructureId = Guid.NewGuid();
                structure.Status = "Active";
                structure.CreatedAt = DateTime.UtcNow;
                structure.UpdatedAt = DateTime.UtcNow;
            }

            await _dbContext.GradingStructures.AddRangeAsync(gradingStructures);

            return true;
        }
    }
}
