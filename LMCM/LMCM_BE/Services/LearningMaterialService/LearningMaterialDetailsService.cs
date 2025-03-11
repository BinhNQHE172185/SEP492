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
        public Task<LearningMaterialDetail> InsertMaterialDetailsAsync(LearningMaterialDetailsInsertDto detail)
        {
            return _materialDetailRepository.InsertMaterialDetailsAsync(detail);
        }
    }
}
