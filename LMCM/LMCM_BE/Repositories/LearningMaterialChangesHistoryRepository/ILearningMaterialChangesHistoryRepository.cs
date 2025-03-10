using LMCM_BE.DTOs.LearningMaterialChangesHistoryProfilesDtos;
using LMCM_BE.DTOs.ShareDtos;

namespace LMCM_BE.Repositories.LearningMaterialChangesHistoryRepository
{
    public interface ILearningMaterialChangesHistoryRepository
    {
        Task<PagedResult<ChangesHistoryListDto>> GetChangesHistoriesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
    }
}
