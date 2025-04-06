using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;
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

        public async Task<LearningMaterialViewDto> GetLearningMaterialByIdAsync(Guid materialId)
        {
            return await _materialRepository.GetLearningMaterialByIdAsync(materialId);
        }

        public async Task<PagedResult<LearningMaterialListDto>> GetMaterialsBySyllabusIdAsync(Guid syllabusId,string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            return await _materialRepository.GetMaterialsBySyllabusIdAsync(syllabusId,searchKey, pageIndex, pageSize);
        }

        public async Task<List<LearningMaterialListDto>> GetMaterialsBySyllabusIdAsync(Guid syllabusId)
        {
            return await _materialRepository.GetMaterialsBySyllabusIdAsync(syllabusId);
        }

        public async Task<Guid?> InsertLearningMaterialAsync(LearningMaterialInsertDto material)
        {
            return await _materialRepository.InsertLearningMaterialAsync(material); 
        }

        public async Task<Guid?> UpdateLearningMaterialAsync(Guid materialId, LearningMaterialUpdateDto newMaterial)
        {
            return await _materialRepository.UpdateLearningMaterialAsync(materialId, newMaterial);
        }
        public async Task<List<string>> GetPublishersAsync()
        {
            return await _materialRepository.GetPublishersAsync();
        }
    }
}
