using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;

namespace LMCM_BE.Services.LearningMaterialService
{
    public interface ILearningMaterialChangesHistorySerivce
    {
        Task<PagedResult<ChangesHistoryListDto>> GetChangesHistoriesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
    }
}
