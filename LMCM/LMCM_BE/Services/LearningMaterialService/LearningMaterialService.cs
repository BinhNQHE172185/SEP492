using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.Models;
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

        public async Task<bool> DeleteLearningMaterialByIdAsync(Guid materialId)
        {
            return await _materialRepository.DeleteLearningMaterialByIdAsync(materialId);
        }

        public async Task<bool> DeleteLearningMaterialsBySyllabusAsync(Guid syllabusId)
        {
            return await _materialRepository.DeleteLearningMaterialsBySyllabusAsync(syllabusId);
        }

        public async Task<LearningMaterial> GetLearningMaterialByIdAsync(Guid materialId)
        {
            return await _materialRepository.GetLearningMaterialByIdAsync(materialId);
        }

        public async Task<bool> ImportLearningMaterialsAsync(List<LearningMaterialImportDto> materials)
        {
            return await _materialRepository.ImportLearningMaterialsAsync(materials);
        }

        public async Task<LearningMaterial> InsertLearningMaterialAsync(LearningMaterialInsertDto material)
        {
            return await _materialRepository.InsertLearningMaterialAsync(material); 
        }

        public async Task<bool> UpdateLearningMaterialAsync(Guid materialId, LearningMaterialUpdateDto newMaterial)
        {
            return await _materialRepository.UpdateLearningMaterialAsync(materialId, newMaterial);
        }
    }
}
