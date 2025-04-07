using LMCM_BE.DTOs.AcceptanceRecordDtos;
using LMCM_BE.DTOs.ShareDtos;

namespace LMCM_BE.Services.AcceptanceRecordService
{
    public interface IAcceptanceRecordService
    {
        Task<PagedResult<AcceptanceRecordListDto>> GetAcceptanceRecordsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<bool> CreateAcceptanceRecordAsync(AcceptanceRecordCreateDto dto);
        Task<Guid?> UpdateAcceptanceRecordAsync(Guid acceptanceId, AcceptanceRecordUpdateDto dto);
        Task<bool> SoftDeleteAcceptanceRecordAsync(Guid acceptanceId);
        Task<AcceptanceRecordDetailDto> GetAcceptanceRecordDetailAsync(Guid acceptanceId);
    }
}
