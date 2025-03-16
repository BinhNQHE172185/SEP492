using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Services.LearningMaterialService
{
    public interface ILearningMaterialService
    {
        Task<bool> ImportLearningMaterialsAsync(List<LearningMaterialImportDto> materials);
        Task<bool> DeleteLearningMaterialsBySyllabusAsync(Guid syllabusId);
        Task<LearningMaterial> InsertLearningMaterialAsync(LearningMaterialInsertDto material);
        Task<LearningMaterial> GetLearningMaterialByIdAsync(Guid materialId);
        Task<bool> UpdateLearningMaterialAsync(Guid materialId, LearningMaterialUpdateDto newMaterial);
        Task<bool> DeleteLearningMaterialByIdAsync(Guid materialId);
    }
}
