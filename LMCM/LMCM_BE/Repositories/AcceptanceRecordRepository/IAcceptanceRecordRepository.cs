using LMCM_BE.DTOs.AcceptanceRecordDtos;
using LMCM_BE.DTOs.ShareDtos;

namespace LMCM_BE.Repositories.AcceptanceRecordRepository
{
    public interface IAcceptanceRecordRepository
    {
        Task<PagedResult<AcceptanceRecordListDto>> GetAcceptanceRecordsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<AcceptanceRecordDetailDto> CreateAcceptanceRecordAsync(AcceptanceRecordCreateDto dto);
        Task<Guid?> UpdateAcceptanceRecordAsync(Guid acceptanceId, AcceptanceRecordUpdateDto dto);
        Task<bool> SoftDeleteAcceptanceRecordAsync(Guid acceptanceId);
        Task<AcceptanceRecordDetailDto> GetAcceptanceRecordDetailAsync(Guid acceptanceId);
    }
}
