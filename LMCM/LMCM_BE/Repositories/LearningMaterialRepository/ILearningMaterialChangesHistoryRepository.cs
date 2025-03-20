using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Repositories.LearningMaterialRepository
{
    public interface ILearningMaterialChangesHistoryRepository
    {
        Task<PagedResult<ChangesHistoryListDto>> GetChangesHistoriesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<bool> CreateLearningMaterialChangesHistoryAsync(CreateLearningMaterialChangesHistoryDto historyDto);
        Task<PagedResult<ChangesHistoryWithMaterialDto>> GetLearningMaterialChangeHistoriesAsync(
     Guid? learningMaterialId, string? searchKey, int pageIndex = 1, int pageSize = 10);
    }
}
