using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Services.LearningMaterialService
{
    public interface ILearningMaterialDetailsService
    {
        Task<LearningMaterialDetail> InsertMaterialDetailsAsync(LearningMaterialDetailsInsertDto detail);
        Task<LearningMaterialDetail> GetMaterialDetailByIdAsync(Guid materialDetailId);
        Task<bool> UpdateMaterialDetailAsync(Guid materialDetailId, LearningMaterialDetailsInsertDto newDetail);
        Task<bool> DeleteMaterialDetailByIdAsync(Guid materialDetailId);
    }
}
