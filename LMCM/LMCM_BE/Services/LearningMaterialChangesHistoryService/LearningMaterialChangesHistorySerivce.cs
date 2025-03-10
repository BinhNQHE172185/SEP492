using LMCM_BE.DTOs.LearningMaterialChangesHistoryProfilesDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Repositories.LearningMaterialChangesHistoryRepository;

namespace LMCM_BE.Services.LearningMaterialChangesHistoryService
{
    public class LearningMaterialChangesHistorySerivce : ILearningMaterialChangesHistorySerivce
    {
        private readonly ILearningMaterialChangesHistoryRepository _changesRepository;
        public LearningMaterialChangesHistorySerivce(ILearningMaterialChangesHistoryRepository changesRepository)
        {
            _changesRepository = changesRepository;
        }

        public async Task<PagedResult<ChangesHistoryListDto>> GetChangesHistoriesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var data = await _changesRepository.GetChangesHistoriesAsync(searchKey, pageIndex, pageSize);
            return data;
        }
    }
}
