using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Repositories.LearningMaterialRepository
{
    public interface ILearningMaterialDetailsRepository
    {
        Task<LearningMaterialDetail> InsertMaterialDetailsAsync(LearningMaterialDetailsInsertDto detail);
    }
}
