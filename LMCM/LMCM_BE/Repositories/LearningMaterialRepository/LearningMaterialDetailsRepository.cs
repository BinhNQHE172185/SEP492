using AutoMapper;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;

namespace LMCM_BE.Repositories.LearningMaterialRepository
{
    public class LearningMaterialDetailsRepository : ILearningMaterialDetailsRepository
    {
        private readonly LMCM_DBContext _dbContext;
        private readonly IMapper _mapper;
        public LearningMaterialDetailsRepository(LMCM_DBContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<bool> DeleteMaterialDetailByIdAsync(Guid materialDetailId)
        {
            if (materialDetailId == Guid.Empty)
                throw new ArgumentException("Material ID cannot be empty.", nameof(materialDetailId));

            try
            {
                var materialDetail = await GetMaterialDetailByIdAsync(materialDetailId);

                if (materialDetail == null)
                    return false; // No material found 

                materialDetail.Status = "Inactive";
                materialDetail.UpdatedAt = DateTime.UtcNow;

                _dbContext.LearningMaterialDetails.Update(materialDetail);
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

        public  async Task<LearningMaterialDetail> GetMaterialDetailByIdAsync(Guid materialDetailId)
        {
            if (materialDetailId == Guid.Empty)
                throw new ArgumentException("Material ID cannot be empty.", nameof(materialDetailId));

            LearningMaterialDetail learningMateriaDetaill = await _dbContext.LearningMaterialDetails.FindAsync(materialDetailId);

            if (learningMateriaDetaill == null)
                throw new KeyNotFoundException($"Learning material detail with ID {materialDetailId} was not found.");

            return learningMateriaDetaill;
        }
        public async Task<PagedResult<LearningMaterialDetailDto>> GetMaterialDetailsListAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.LearningMaterialDetails
                .Where(detail => detail.Status == "Active")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                query = query.Where(detail => detail.MaterialName.ToLower().Contains(search) ||
                                              detail.MaterialDescription.ToLower().Contains(search));
            }

            int totalCount = await query.CountAsync();

            var items = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            var data = _mapper.Map<List<LearningMaterialDetailDto>>(items);

            return new PagedResult<LearningMaterialDetailDto>
            {
                Items = data,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<List<string>> GetPublishersAsync()
        {
            var publishers = await _dbContext.LearningMaterialDetails
                .Where(lm => !string.IsNullOrEmpty(lm.Publisher))
                .Select(lm => lm.Publisher)
                .Distinct()
                .ToListAsync();

            return publishers;
        }

        public async Task<LearningMaterialDetail> InsertMaterialDetailsAsync(LearningMaterialDetailsInsertDto detail)
        {
            if (detail == null)
                throw new ArgumentNullException(nameof(detail));

            var newDetail = _mapper.Map<LearningMaterialDetail>(detail);

            newDetail.MaterialDetailId = Guid.NewGuid();
            newDetail.Status = "Active";
            newDetail.CreatedAt = DateTime.UtcNow;
            newDetail.UpdatedAt = DateTime.UtcNow;

            await _dbContext.LearningMaterialDetails.AddAsync(newDetail);

            return newDetail;
        }

        public async Task<bool> UpdateMaterialDetailAsync(Guid materialDetailId, LearningMaterialDetailsInsertDto newDetail)
        {
            if (materialDetailId == null)
                throw new ArgumentNullException(nameof(materialDetailId), "material id cannot be null.");

            if (newDetail == null)
                throw new ArgumentNullException(nameof(newDetail), "New material data cannot be null.");

            LearningMaterialDetail learningMaterialDetail= await GetMaterialDetailByIdAsync(materialDetailId);

            if (learningMaterialDetail == null)
                throw new ArgumentNullException(nameof(learningMaterialDetail), "material data not found.");

            // Use AutoMapper to update existing entity
            _mapper.Map(newDetail, learningMaterialDetail);
            learningMaterialDetail.UpdatedAt = DateTime.UtcNow;

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
