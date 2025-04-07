using LMCM_BE.Models;

namespace LMCM_BE.Repositories.LearningMaterialRepository
{
    public interface ILearningMaterialRepository
    {
        Task<(List<LearningMaterial>,int totalCount)> GetMaterialsBySyllabusIdAsync(Guid syllabusId,string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<List<LearningMaterial>> GetMaterialsBySyllabusIdAsync(Guid syllabusId);
        Task<bool> ImportLearningMaterialsAsync(List<LearningMaterial> materials, Guid? oldSyllabusId, Guid newSyllabusId, bool keepUserCreated);
        Task<bool> DeleteLearningMaterialsBySyllabusAsync(Guid syllabusId);
        Task<bool> InsertLearningMaterialAsync(LearningMaterial material);
        Task<LearningMaterial> GetLearningMaterialByIdAsync(Guid materialId);
        Task<bool> UpdateLearningMaterialAsync(LearningMaterial newMaterial);
        Task<bool> DeleteLearningMaterialAsync(LearningMaterial material);
        Task<List<string>> GetPublishersAsync();
    }
}
