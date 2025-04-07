using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Services.LearningMaterialService
{
    public interface ILearningMaterialService
    {
        Task<PagedResult<LearningMaterialListDto>> GetMaterialsBySyllabusIdAsync(Guid syllabusId, string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<List<LearningMaterialListDto>> GetMaterialsBySyllabusIdAsync(Guid syllabusId);
        Task<bool> InsertLearningMaterialAsync(LearningMaterialInsertDto material);
        Task<bool> ImportLearningMaterialsAsync(List<LearningMaterial> materials, Guid? oldSyllabusId, Guid newSyllabusId, bool keepUserCreated);
        Task<LearningMaterialViewDto> GetLearningMaterialByIdAsync(Guid materialId);
        Task<bool> UpdateLearningMaterialAsync(Guid materialId, LearningMaterialUpdateDto newMaterial);
        Task<bool> DeleteLearningMaterialByIdAsync(Guid materialId);
        Task<bool> DeleteLearningMaterialsBySyllabusAsync(Guid syllabusId);
        Task<List<string>> GetPublishersAsync();
    }
}
