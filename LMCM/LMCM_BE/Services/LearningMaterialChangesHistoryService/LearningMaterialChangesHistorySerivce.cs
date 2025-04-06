using LMCM_BE.DTOs.LearningMaterialChangesHistoryDtos;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
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
        public async Task<bool> CreateLearningMaterialChangesHistoryAsync(CreateLearningMaterialChangesHistoryDto historyDto)
        {
            return await _changesRepository.CreateLearningMaterialChangesHistoryAsync(historyDto);
        }
        public async Task<PagedResult<ChangesHistoryOfSubjectDto>> GetLearningMaterialChangesHistoriesOfSubjectAsync(
     Guid? subjectId, string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            return await _changesRepository.GetLearningMaterialChangesHistoriesOfSubjectAsync(subjectId, searchKey, pageIndex, pageSize);
        }
        public async Task<bool> SoftDeleteLearningMaterialChangesHistoryAsync(Guid historyId)
        {
            return await _changesRepository.SoftDeleteLearningMaterialChangesHistoryAsync(historyId);
        }
        public async Task<Guid?> UpdateLearningMaterialChangesHistoryAsync(Guid historyId, UpdateLearningMaterialChangesHistoryDto dto)
        {
            return await _changesRepository.UpdateLearningMaterialChangesHistoryAsync(historyId, dto);
        }

        public async Task<ChangesHistoryDetailDto> getHistoryOfChangeDetail(Guid id)
        {
            return await _changesRepository.getHistoryOfChangeDetail(id);
        }
    }
}
