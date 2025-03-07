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

        public async Task<bool> ImportGradingStructuresAsync(List<GradingStructureInsertDto> gradingStructures)
        {
            if (gradingStructures == null || !gradingStructures.Any())
                throw new ArgumentNullException(nameof(gradingStructures));

            var newGradingStructures = gradingStructures.Select(gradingStructureDto => new GradingStructure
            {
                StructureId = Guid.NewGuid(),
                SyllabusId = gradingStructureDto.SyllabusId,
                StructureNo = gradingStructureDto.StructureNo,
                AssessmentComponent = gradingStructureDto.AssessmentComponent,
                AssessmentType = gradingStructureDto.AssessmentType,
                Weight = gradingStructureDto.Weight,    
                Part= gradingStructureDto.Part,
                MinValue= gradingStructureDto.MinValue,
                Duration= gradingStructureDto.Duration,
                Clo= gradingStructureDto.Clo,
                QuestionType= gradingStructureDto.QuestionType, 
                QuestionNo= gradingStructureDto.QuestionNo, 
                Scope= gradingStructureDto.Scope,
                How= gradingStructureDto.How,
                Note= gradingStructureDto.Note,
                SessionNo= gradingStructureDto.SessionNo,
                Reference= gradingStructureDto.Reference,
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToList();

            await _dbContext.GradingStructures.AddRangeAsync(newGradingStructures);
            return true;
        }
    }
}
