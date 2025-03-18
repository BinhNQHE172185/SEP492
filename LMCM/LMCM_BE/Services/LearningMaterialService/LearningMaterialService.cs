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

        public async Task<bool> DeleteLearningMaterialsBySyllabusAsync(Guid syllabusId)
        {
            return await _materialRepository.DeleteLearningMaterialsBySyllabusAsync(syllabusId);
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

        public async Task<bool> ImportLearningMaterialsAsync(List<LearningMaterialImportDto> materials, Guid? oldSyllabusId, Guid newSyllabusId)
        {
            return await _materialRepository.ImportLearningMaterialsAsync(materials,oldSyllabusId,newSyllabusId);
        }

        public async Task<Guid?> InsertLearningMaterialAsync(LearningMaterialInsertDto material)
        {
            return await _materialRepository.InsertLearningMaterialAsync(material); 
        }

        public async Task<Guid?> UpdateLearningMaterialAsync(Guid materialId, LearningMaterialUpdateDto newMaterial,bool createChangeHistory)
        {
            return await _materialRepository.UpdateLearningMaterialAsync(materialId, newMaterial,createChangeHistory);
        }
    }
}
