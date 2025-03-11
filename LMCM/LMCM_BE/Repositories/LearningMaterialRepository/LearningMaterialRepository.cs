using AutoMapper;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.LearningMaterialRepository
{
    public class LearningMaterialRepository : ILearningMaterialRepository
    {
        private readonly LMCM_DBContext _dbContext;
        private readonly IMapper _mapper;
        public LearningMaterialRepository(LMCM_DBContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<bool> DeleteLearningMaterialsBySyllabusAsync(Guid syllabusId)
        {
            if (syllabusId == Guid.Empty)
                throw new ArgumentException("Syllabus ID cannot be empty.", nameof(syllabusId));

            try
            {
                var materials = await _dbContext.LearningMaterials
                    .Where(s => s.SyllabusId == syllabusId)
                    .ToListAsync();

                if (!materials.Any())
                    return false; // No schedules found for the given syllabus

                foreach (var material in materials)
                {
                    material.Status = "Inactive";
                    material.UpdatedAt = DateTime.UtcNow;
                }

                _dbContext.LearningMaterials.UpdateRange(materials);
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

        public async Task<bool> ImportLearningMaterialsAsync(List<LearningMaterialInsertDto> materials)
        {
            if (materials == null || !materials.Any())
                throw new ArgumentNullException(nameof(materials));

            var newMaterials = materials.Select(materialDto => new LearningMaterial
            {
                MaterialId = Guid.NewGuid(),
                SyllabusId=materialDto.SyllabusId,
                MaterialDetailId = materialDto.MaterialDetailId,
                LearningType=materialDto.LearningType,
                MaterialType=materialDto.MaterialType,
                IsMainMaterial=false,
                MaterialNo=materialDto.MaterialNo,  
                MaterialName=materialDto.MaterialName,
                MaterialQuantity=materialDto.MaterialQuantity,
                Url=materialDto.Url,
                Purpose=materialDto.Purpose,
                Note=materialDto.Note,
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToList();

            await _dbContext.LearningMaterials.AddRangeAsync(newMaterials);
            return true;
        }
    }
}
