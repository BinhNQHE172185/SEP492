using LMCM_BE.DbContext;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.LearningMaterialRepository
{
    public class LearningMaterialRepository : ILearningMaterialRepository
    {
        private readonly LMCM_DBContext _dbContext;
        public LearningMaterialRepository(LMCM_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> DeleteLearningMaterialsBySyllabusAsync(Guid syllabusId)
        {
            var materials = await _dbContext.LearningMaterials
                .Where(s => s.SyllabusId == syllabusId)
                .ToListAsync();

            foreach (var material in materials)
            {
                material.Status = "Inactive";
                material.UpdatedAt = DateTime.UtcNow;
            }

            _dbContext.LearningMaterials.UpdateRange(materials);

            return true;
        }

        public async Task<LearningMaterial> GetLearningMaterialByIdAsync(Guid materialId)
        {

            var learningMaterial = await _dbContext.LearningMaterials
                .FirstOrDefaultAsync(s => s.MaterialId == materialId);

            return learningMaterial;
        }

        public async Task<(List<LearningMaterial>,int totalCount)> GetMaterialsBySyllabusIdAsync(Guid syllabusId, string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.LearningMaterials.AsQueryable();

            query = query.Where(s => s.SyllabusId == syllabusId && s.Status != "Deleted");

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                query = query.Where(s => s.MaterialName.ToLower().Contains(search));
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return(items,totalCount);   
        }

        public async Task<List<LearningMaterial>> GetMaterialsBySyllabusIdAsync(Guid syllabusId)
        {
            var query = _dbContext.LearningMaterials.AsQueryable();

            var items = await query.Where(s => s.SyllabusId == syllabusId && s.Status != "Deleted")
                .OrderByDescending(s => s.UpdatedAt)
                .ToListAsync();

            return items;
        }

        public async Task<bool> ImportLearningMaterialsAsync(List<LearningMaterial> materials, Guid? oldSyllabusId, Guid newSyllabusId, bool keepUserCreated)
        {
            if (materials == null || !materials.Any())
                return false;

            foreach (var material in materials)
            {
                material.SyllabusId = newSyllabusId;
                material.MaterialId = Guid.NewGuid();
                material.IsMainMaterial = false;
                material.IsImportedMaterial = true;
                material.MaterialType = "Học Liệu";
                material.Status = "Active";
                material.CreatedAt = DateTime.UtcNow;
                material.UpdatedAt = DateTime.UtcNow;
            }

            if (oldSyllabusId != null && keepUserCreated)
            {
                var existingMaterials = await _dbContext.LearningMaterials
                    .Where(s => s.SyllabusId == oldSyllabusId && s.Status == "Active" && s.IsImportedMaterial == false)
                    .ToListAsync();
                foreach (var material in existingMaterials)
                {
                    var newMaterial = new LearningMaterial
                    {
                        MaterialId = Guid.NewGuid(),  // Create a new MaterialId
                        SyllabusId = newSyllabusId,   // Assign the new syllabus
                        LearningType = material.LearningType,
                        MaterialType = material.MaterialType,
                        IsMainMaterial = material.IsMainMaterial,
                        IsImportedMaterial = false,    // Mark as imported material
                        MaterialName = material.MaterialName,
                        Isbn = material.Isbn,
                        Author = material.Author,
                        Publisher = material.Publisher,
                        PublishedDate = material.PublishedDate,
                        Edition = material.Edition,
                        Url = material.Url,
                        Purpose = material.Purpose,
                        Note = material.Note,
                        Status = "Active",
                        CreatedAt = DateTime.UtcNow,  // Set created date to current time
                        UpdatedAt = DateTime.UtcNow  // Set updated date to current time
                    };

                    // Add the new material copy to the new materials list
                    materials.Add(newMaterial);
                }
            }


            await _dbContext.LearningMaterials.AddRangeAsync(materials);
            return true;
        }

        public async Task<bool> InsertLearningMaterialAsync(LearningMaterial material)
        {
            var newMaterial=material;
            newMaterial.MaterialId = Guid.NewGuid();
            newMaterial.IsImportedMaterial = false;
            newMaterial.Status = "Active";
            newMaterial.CreatedAt = DateTime.UtcNow;
            newMaterial.UpdatedAt = DateTime.UtcNow;

            await _dbContext.LearningMaterials.AddAsync(newMaterial);

            return true;
        }

        public async Task<bool> UpdateLearningMaterialAsync(LearningMaterial newMaterial)
        {
            var learningMaterial=newMaterial;
            learningMaterial.UpdatedAt = DateTime.UtcNow;
            _dbContext.LearningMaterials.Update(learningMaterial);
            return true;

        }
        public async Task<List<string>> GetPublishersAsync()
        {
            var publishers = await _dbContext.LearningMaterials
                .Where(lm => !string.IsNullOrEmpty(lm.Publisher))
                .Select(lm => lm.Publisher)
                .Distinct()
                .ToListAsync();

            return publishers;
        }
    }
}
