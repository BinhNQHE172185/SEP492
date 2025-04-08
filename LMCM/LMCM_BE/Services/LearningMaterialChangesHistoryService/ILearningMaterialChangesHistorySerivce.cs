using LMCM_BE.DTOs.LearningMaterialChangesHistoryDtos;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Services.LearningMaterialChangesHistoryService
{
    public interface ILearningMaterialChangesHistorySerivce
    {
        Task<PagedResult<ChangesHistoryListDto>> GetChangesHistoriesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<bool> CreateLearningMaterialChangesHistoryAsync(CreateLearningMaterialChangesHistoryDto historyDto);
        Task<PagedResult<ChangesHistoryOfSubjectDto>> GetLearningMaterialChangesHistoriesOfSubjectAsync(
     Guid? subjectId, string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<bool> SoftDeleteLearningMaterialChangesHistoryAsync(Guid historyId);
        Task<Guid?> UpdateLearningMaterialChangesHistoryAsync(Guid historyId, UpdateLearningMaterialChangesHistoryDto dto);
        Task<ChangesHistoryDetailDto> getHistoryOfChangeDetail(Guid id);
        Task<List<LearningMaterialChangesHistory>> GetAllWithCompletionDateAsync();
    }
}
