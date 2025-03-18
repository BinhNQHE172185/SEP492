using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Repositories.LearningMaterialRepository
{
    public interface ILearningMaterialDetailsRepository
    {
        Task<LearningMaterialDetail> InsertMaterialDetailsAsync(LearningMaterialDetailsInsertDto detail);
        Task<LearningMaterialDetail> GetMaterialDetailByIdAsync(Guid materialDetailId);
        Task<bool> UpdateMaterialDetailAsync(Guid materialDetailId, LearningMaterialDetailsInsertDto newDetail);
        Task<bool> DeleteMaterialDetailByIdAsync(Guid materialDetailId);
        Task<PagedResult<LearningMaterialDetailDto>> GetMaterialDetailsListAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
    }
}
