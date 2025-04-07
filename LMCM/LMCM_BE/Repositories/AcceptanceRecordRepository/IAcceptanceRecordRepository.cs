using LMCM_BE.DTOs.AcceptanceRecordDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Repositories.AcceptanceRecordRepository
{
    public interface IAcceptanceRecordRepository
    {
        Task<(List<AcceptanceRecord>, int totalCount)> GetAcceptanceRecordsAsync(bool isHod, Guid userId, string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<bool> CreateAcceptanceRecordAsync(AcceptanceRecord acceptanceRecord);
        Task<bool> UpdateAcceptanceRecordAsync(AcceptanceRecord acceptanceRecord);
        Task<bool> SoftDeleteAcceptanceRecordAsync(AcceptanceRecord acceptanceRecord);
        Task<bool> HasActiveAcceptanceRecordsAsync(Guid contractId);
        Task<AcceptanceRecord?> GetAcceptanceRecordByIdAsync(Guid acceptanceId);
        Task<AcceptanceRecord?> GetActiveAcceptanceRecordByIdAsync(Guid acceptanceId);
        Task<AcceptanceRecord?> GetAcceptanceRecordDetailAsync(Guid acceptanceId);
    }
}
