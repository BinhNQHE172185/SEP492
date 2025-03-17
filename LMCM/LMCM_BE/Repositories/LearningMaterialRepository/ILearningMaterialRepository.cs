using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Repositories.LearningMaterialRepository
{
    public interface ILearningMaterialRepository
    {
        Task<PagedResult<LearningMaterialListDto>> GetMaterialsBySyllabusIdAsync(Guid syllabusId,string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<List<LearningMaterialListDto>> GetMaterialsBySyllabusIdAsync(Guid syllabusId);
        Task<bool> ImportLearningMaterialsAsync(List<LearningMaterialImportDto> materials);
        Task<bool> DeleteLearningMaterialsBySyllabusAsync(Guid syllabusId);
        Task<Guid?> InsertLearningMaterialAsync(LearningMaterialInsertDto material);
        Task<LearningMaterialViewDto> GetLearningMaterialByIdAsync(Guid materialId);
        Task<Guid?> UpdateLearningMaterialAsync(Guid materialId,LearningMaterialUpdateDto newMaterial,bool createChangeHistory);
        Task<bool> DeleteLearningMaterialByIdAsync(Guid materialId);
    }
}
