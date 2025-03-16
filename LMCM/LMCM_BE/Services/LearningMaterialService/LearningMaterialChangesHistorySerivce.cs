using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.LearningMaterialRepository;

namespace LMCM_BE.Services.LearningMaterialService
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
        public async Task<bool> CreateLearningMaterialChangesHistoryAsync(CreateLearningMaterialChangesHistoryDto historyDto)
        {
            return await _changesRepository.CreateLearningMaterialChangesHistoryAsync(historyDto);
        }
    }
}
