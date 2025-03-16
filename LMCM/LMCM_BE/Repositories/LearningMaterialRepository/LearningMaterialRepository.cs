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
        public LearningMaterialRepository(LMCM_DBContext dbContext, IMapper mapper, ILearningMaterialDetailsRepository detailRepository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<bool> DeleteLearningMaterialByIdAsync(Guid materialId)
        {
            if (materialId == Guid.Empty)
                throw new ArgumentException("Material ID cannot be empty.", nameof(materialId));

            try
            {
                var material = await GetLearningMaterialByIdAsync(materialId);

                if (material == null)
                    return false; // No material found 

                material.Status = "Inactive";
                material.UpdatedAt = DateTime.UtcNow;

                _dbContext.LearningMaterials.Update(material);
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

        public async Task<LearningMaterial> GetLearningMaterialByIdAsync(Guid materialId)
        {
            if (materialId == Guid.Empty)
                throw new ArgumentException("Material ID cannot be empty.", nameof(materialId));

            var learningMaterial = await _dbContext.LearningMaterials
                .Include(s => s.MaterialDetail) 
                .FirstOrDefaultAsync(s => s.MaterialId == materialId);

            if (learningMaterial == null)
                throw new KeyNotFoundException($"Learning material with ID {materialId} was not found.");

            return learningMaterial;
        }


        public async Task<bool> ImportLearningMaterialsAsync(List<LearningMaterialImportDto> materials)
        {
            if (materials == null || !materials.Any())
                throw new ArgumentNullException(nameof(materials));

            var newMaterials = _mapper.Map<List<LearningMaterial>>(materials);

            foreach (var material in newMaterials)
            {
                material.MaterialId = Guid.NewGuid();
                material.IsMainMaterial = false;
                material.Status = "Active";
                material.CreatedAt = DateTime.UtcNow;
                material.UpdatedAt = DateTime.UtcNow;
            }

            await _dbContext.LearningMaterials.AddRangeAsync(newMaterials);

            return true;
        }

        public async Task<LearningMaterial> InsertLearningMaterialAsync(LearningMaterialInsertDto material)
        {
            if (material == null)
                throw new ArgumentNullException(nameof(material));

            var newMaterial = _mapper.Map<LearningMaterial>(material);

            if(newMaterial.MaterialDetail != null)
            {
                newMaterial.MaterialDetail.Status = "Active";
                newMaterial.MaterialDetail.CreatedAt = DateTime.UtcNow;
                newMaterial.MaterialDetail.UpdatedAt = DateTime.UtcNow;
            }

            newMaterial.MaterialId = Guid.NewGuid();
            newMaterial.Status = "Active";
            newMaterial.CreatedAt = DateTime.UtcNow;
            newMaterial.UpdatedAt = DateTime.UtcNow;

            await _dbContext.LearningMaterials.AddAsync(newMaterial);
            await _dbContext.SaveChangesAsync();

            return newMaterial;
        }

        public async Task<bool> UpdateLearningMaterialAsync(Guid materialId, LearningMaterialUpdateDto newMaterial)
        {
            if (materialId == null)
                throw new ArgumentNullException(nameof(materialId), "material id cannot be null.");

            if (newMaterial == null)
                throw new ArgumentNullException(nameof(newMaterial), "New material data cannot be null.");

            LearningMaterial learningMaterial = await GetLearningMaterialByIdAsync(materialId);

            if (learningMaterial == null)
                throw new ArgumentNullException(nameof(learningMaterial), "material data not found.");

            // Use AutoMapper to update existing entity
            _mapper.Map(newMaterial, learningMaterial);
            learningMaterial.UpdatedAt = DateTime.UtcNow;

            if(learningMaterial.MaterialDetail != null)
            {
                learningMaterial.MaterialDetail.UpdatedAt= DateTime.UtcNow; 
            }

            try
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
