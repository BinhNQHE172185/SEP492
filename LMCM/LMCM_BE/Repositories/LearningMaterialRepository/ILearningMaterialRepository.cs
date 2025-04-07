using LMCM_BE.Models;

namespace LMCM_BE.Repositories.LearningMaterialRepository
{
    public interface ILearningMaterialRepository
    {
        Task<(List<LearningMaterial>,int totalCount)> GetMaterialsBySyllabusIdAsync(Guid syllabusId,string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<List<LearningMaterial>> GetMaterialsBySyllabusIdAsync(Guid syllabusId);
        Task<List<LearningMaterial>> GetUserCreatedMaterialsFromSyllabusIdAsync(Guid syllabusId);
        Task<bool> AddMaterialsAsync(List<LearningMaterial> materials);        Task<bool> InsertLearningMaterialAsync(LearningMaterial material);
        Task<LearningMaterial> GetLearningMaterialByIdAsync(Guid materialId);
        Task<bool> UpdateLearningMaterialAsync(LearningMaterial newMaterial);
        Task<bool> UpdateLearningMaterialsAsync(List<LearningMaterial> materials);
        Task<List<string>> GetPublishersAsync();
    }
}
