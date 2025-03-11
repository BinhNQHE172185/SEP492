using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Services.LearningMaterialService
{
    public interface ILearningMaterialDetailsService
    {
        Task<LearningMaterialDetail> InsertMaterialDetailsAsync(LearningMaterialDetailsInsertDto detail);
    }
}
