using LMCM_BE.DTOs.LearningMaterialChangesHistoryProfilesDtos;
using LMCM_BE.DTOs.ShareDtos;

namespace LMCM_BE.Services.LearningMaterialChangesHistoryService
{
    public interface ILearningMaterialChangesHistorySerivce
    {
        Task<PagedResult<ChangesHistoryListDto>> GetChangesHistoriesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
    }
}
