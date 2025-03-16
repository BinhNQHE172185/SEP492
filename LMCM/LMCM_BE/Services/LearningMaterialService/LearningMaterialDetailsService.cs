using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.LearningMaterialRepository;

namespace LMCM_BE.Services.LearningMaterialService
{
    public class LearningMaterialDetailsService : ILearningMaterialDetailsService
    {
        private readonly ILearningMaterialDetailsRepository _materialDetailRepository;
        public LearningMaterialDetailsService(ILearningMaterialDetailsRepository materialDetailRepository)
        {
            _materialDetailRepository = materialDetailRepository;
        }

        public async Task<bool> DeleteMaterialDetailByIdAsync(Guid materialDetailId)
        {
            return await _materialDetailRepository.DeleteMaterialDetailByIdAsync(materialDetailId);
        }

        public async Task<LearningMaterialDetail> GetMaterialDetailByIdAsync(Guid materialDetailId)
        {
            return await _materialDetailRepository.GetMaterialDetailByIdAsync(materialDetailId);
        }

        public Task<LearningMaterialDetail> InsertMaterialDetailsAsync(LearningMaterialDetailsInsertDto detail)
        {
            return _materialDetailRepository.InsertMaterialDetailsAsync(detail);
        }

        public async Task<bool> UpdateMaterialDetailAsync(Guid materialDetailId, LearningMaterialDetailsInsertDto newDetail)
        {
            return await _materialDetailRepository.UpdateMaterialDetailAsync(materialDetailId, newDetail);  
        }
    }
}
