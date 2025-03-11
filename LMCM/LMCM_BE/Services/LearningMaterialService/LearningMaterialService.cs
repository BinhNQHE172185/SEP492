using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.Repositories.LearningMaterialRepository;

namespace LMCM_BE.Services.LearningMaterialService
{
    public class LearningMaterialService : ILearningMaterialService
    {
        private readonly ILearningMaterialRepository _materialRepository;
        public LearningMaterialService(ILearningMaterialRepository materialRepository)
        {
            _materialRepository = materialRepository;
        }
        public async Task<bool> DeleteLearningMaterialsBySyllabusAsync(Guid syllabusId)
        {
            return await _materialRepository.DeleteLearningMaterialsBySyllabusAsync(syllabusId);
        }

        public async Task<bool> ImportLearningMaterialsAsync(List<LearningMaterialInsertDto> materials)
        {
            return await _materialRepository.ImportLearningMaterialsAsync(materials);
        }
    }
}
