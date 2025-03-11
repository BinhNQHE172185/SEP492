using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;

namespace LMCM_BE.Repositories.LearningMaterialRepository
{
    public interface ILearningMaterialChangesHistoryRepository
    {
        Task<PagedResult<ChangesHistoryListDto>> GetChangesHistoriesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
    }
}
