using LMCM_BE.DTOs.LearningMaterialChangesHistoryDtos;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Repositories.LearningMaterialChangesHistoryRepository
{
    public interface ILearningMaterialChangesHistoryRepository
    {
        Task<(List<LearningMaterialChangesHistory>, int totalCount)> GetChangesHistoriesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<bool> CreateLearningMaterialChangesHistoryAsync(LearningMaterialChangesHistory history);
        Task<(List<LearningMaterialChangesHistory>, int totalCount)> GetLearningMaterialChangesHistoriesOfSubjectAsync(
    List<Syllabus> syllabuses, string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<bool> UpdateLearningMaterialChangesHistoryAsync(LearningMaterialChangesHistory history);
        Task<LearningMaterialChangesHistory?> GetActiveHistoryByIdAsync(Guid historyId);
        Task<LearningMaterialChangesHistory?> getHistoryOfChangeDetail(Guid id);
        Task<List<LearningMaterialChangesHistory>> GetAllWithCompletionDateAsync();
    }
}
