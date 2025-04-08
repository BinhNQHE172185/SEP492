using LMCM_BE.DbContext;
using LMCM_BE.Models;
using LMCM_BE.Models.Constant;
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

        public async Task<LearningMaterial?> GetLearningMaterialByIdAsync(Guid materialId)
        {
            return await _dbContext.LearningMaterials
                .FirstOrDefaultAsync(s => s.MaterialId == materialId);
        }

        public async Task<(List<LearningMaterial>, int totalCount)> GetMaterialsBySyllabusIdAsync(Guid syllabusId, string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.LearningMaterials.AsQueryable();

            query = query.Where(s => s.SyllabusId == syllabusId && s.Status != LearningMaterialStatus.Deleted);

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

            return (items, totalCount);
        }

        public async Task<List<LearningMaterial>> GetMaterialsBySyllabusIdAsync(Guid syllabusId)
        {
            return await _dbContext.LearningMaterials
                .Where(m => m.SyllabusId == syllabusId && m.Status != LearningMaterialStatus.Deleted)
                .OrderByDescending(m => m.UpdatedAt)
                .ToListAsync();
        }

        public async Task<List<LearningMaterial>> GetUserCreatedMaterialsFromSyllabusIdAsync(Guid syllabusId)
        {
            return await _dbContext.LearningMaterials
                .Where(s => s.SyllabusId == syllabusId && s.Status == LearningMaterialStatus.Active && s.IsImportedMaterial == false)
                .ToListAsync();
        }
        public async Task<bool> AddMaterialsAsync(List<LearningMaterial> materials)
        {
            await _dbContext.LearningMaterials.AddRangeAsync(materials);

            return true;
        }

        public async Task<bool> InsertLearningMaterialAsync(LearningMaterial material)
        {
            await _dbContext.LearningMaterials.AddAsync(material);

            return true;
        }

        public async Task<bool> UpdateLearningMaterialAsync(LearningMaterial newMaterial)
        {
            _dbContext.LearningMaterials.Update(newMaterial);

            return true;

        }
        public async Task<bool> UpdateLearningMaterialsAsync(List<LearningMaterial> materials)
        {
            _dbContext.LearningMaterials.UpdateRange(materials);

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
