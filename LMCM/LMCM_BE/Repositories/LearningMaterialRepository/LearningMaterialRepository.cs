using AutoMapper;
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
                var learningMaterial = await _dbContext.LearningMaterials
                        .Include(lm => lm.MaterialDetail)
                        .FirstOrDefaultAsync(lm => lm.MaterialId == materialId);

                if (learningMaterial == null)
                    return false; // No material found 

                learningMaterial.Status = "Deleted";
                learningMaterial.UpdatedAt = DateTime.UtcNow;

                _dbContext.LearningMaterials.Update(learningMaterial);
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

        public async Task<LearningMaterialViewDto> GetLearningMaterialByIdAsync(Guid materialId)
        {
            if (materialId == Guid.Empty)
                throw new ArgumentException("Material ID cannot be empty.", nameof(materialId));

            var learningMaterial = await _dbContext.LearningMaterials
                .Include(s => s.MaterialDetail)
                .FirstOrDefaultAsync(s => s.MaterialId == materialId);

            if (learningMaterial == null)
                throw new KeyNotFoundException($"Learning material with ID {materialId} was not found.");

            return _mapper.Map<LearningMaterialViewDto>(learningMaterial);
        }

        public async Task<PagedResult<LearningMaterialListDto>> GetMaterialsBySyllabusIdAsync(Guid syllabusId, string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.LearningMaterials.AsQueryable();

            query = query.OrderBy(s => s.MaterialNo);

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
                .Include(s => s.MaterialDetail)
                .ToListAsync();

            var data = _mapper.Map<List<LearningMaterialListDto>>(items);

            return new PagedResult<LearningMaterialListDto>
            {
                Items = data,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<List<LearningMaterialListDto>> GetMaterialsBySyllabusIdAsync(Guid syllabusId)
        {
            var query = _dbContext.LearningMaterials.AsQueryable();

            query = query.Where(s => s.SyllabusId == syllabusId && s.Status != "Deleted")
                         .Include(s => s.MaterialDetail);

            var items = await query
                .OrderBy(s=>s.MaterialNo)
                .ToListAsync();

            var data = _mapper.Map<List<LearningMaterialListDto>>(items);

            return data;
        }

        public async Task<bool> ImportLearningMaterialsAsync(List<LearningMaterialImportDto> materials, Guid? oldSyllabusId, Guid newSyllabusId)
        {
            if (materials == null || !materials.Any())
                throw new ArgumentNullException(nameof(materials));

            var newMaterials = _mapper.Map<List<LearningMaterial>>(materials);
            foreach (var material in newMaterials)
            {
                material.MaterialId = Guid.NewGuid();
                material.IsMainMaterial = false;
                material.IsImportedMaterial = true;
                material.Status = "Active";
                material.CreatedAt = DateTime.UtcNow;
                material.UpdatedAt = DateTime.UtcNow;
                if (oldSyllabusId != null && material.MaterialType == "Imported Material" && material.MaterialDetailId == null)
                {
                    var oldMaterial = await _dbContext.LearningMaterials
                        .Where(s => s.SyllabusId == oldSyllabusId && s.Url == material.Url)
                        .Include(s => s.MaterialDetail)
                        .FirstOrDefaultAsync();
                    if (oldMaterial != null) material.MaterialDetailId = oldMaterial.MaterialDetailId;
                }
            }

            if (oldSyllabusId != null)
            {
                var existingMaterials = await _dbContext.LearningMaterials
                    .Where(s => s.SyllabusId == oldSyllabusId && s.Status == "Inactive" && s.IsImportedMaterial == false)
                    .ToListAsync();
                foreach (var material in existingMaterials)
                {
                    material.MaterialId = Guid.NewGuid();
                    material.SyllabusId = newSyllabusId;
                    material.Status = "Active";
                    material.CreatedAt = DateTime.UtcNow;
                    material.UpdatedAt = DateTime.UtcNow;
                }

                newMaterials.AddRange(existingMaterials);
            }


            await _dbContext.LearningMaterials.AddRangeAsync(newMaterials);

            return true;
        }

        public async Task<Guid?> InsertLearningMaterialAsync(LearningMaterialInsertDto material)
        {
            if (material == null)
                throw new ArgumentNullException(nameof(material));

            var newMaterial = _mapper.Map<LearningMaterial>(material);

            if (newMaterial.MaterialDetail != null)
            {
                newMaterial.MaterialDetail.Status = "Active";
                newMaterial.MaterialDetail.CreatedAt = DateTime.UtcNow;
                newMaterial.MaterialDetail.UpdatedAt = DateTime.UtcNow;
            }

            newMaterial.MaterialId = Guid.NewGuid();
            newMaterial.IsImportedMaterial = false;
            newMaterial.Status = "Active";
            newMaterial.CreatedAt = DateTime.UtcNow;
            newMaterial.UpdatedAt = DateTime.UtcNow;

            await _dbContext.LearningMaterials.AddAsync(newMaterial);
            await _dbContext.SaveChangesAsync();

            return newMaterial.MaterialId;
        }

        public async Task<Guid?> UpdateLearningMaterialAsync(Guid materialId, LearningMaterialUpdateDto newMaterial)
        {
            if (materialId == null)
                throw new ArgumentNullException(nameof(materialId), "material id cannot be null.");

            if (newMaterial == null)
                throw new ArgumentNullException(nameof(newMaterial), "New material data cannot be null.");

            var learningMaterial = await _dbContext.LearningMaterials
                    .Include(lm => lm.MaterialDetail)
                    .FirstOrDefaultAsync(lm => lm.MaterialId == materialId);

            if (learningMaterial == null)
                throw new ArgumentNullException(nameof(learningMaterial), "material data not found.");

            // Use AutoMapper to update existing entity
            _mapper.Map(newMaterial, learningMaterial);
            learningMaterial.UpdatedAt = DateTime.UtcNow;

            if (learningMaterial.MaterialDetail != null)
            {
                learningMaterial.MaterialDetail.UpdatedAt = DateTime.UtcNow;
            }
            try
            {
                await _dbContext.SaveChangesAsync();
                return learningMaterial.MaterialId;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
