using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Services.LearningMaterialService
{
    public interface ILearningMaterialChangesHistorySerivce
    {
        Task<PagedResult<ChangesHistoryListDto>> GetChangesHistoriesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<bool> CreateLearningMaterialChangesHistoryAsync(CreateLearningMaterialChangesHistoryDto historyDto);
        Task<PagedResult<ChangesHistoryWithMaterialDto>> GetLearningMaterialChangeHistoriesAsync(
     Guid? learningMaterialId, string? searchKey, int pageIndex = 1, int pageSize = 10);
    }
}
