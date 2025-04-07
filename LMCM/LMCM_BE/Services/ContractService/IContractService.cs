using LMCM_BE.DTOs.ContractDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Services.ContractService
{
    public interface IContractService
    {
        Task<bool> CreateContract(UserProfileResponseDto user, ContractInsertDto contractDto);
        Task<ContractDetailDto> GetContractByIdAsync(UserProfileResponseDto user, Guid contractId);
        Task<PagedResult<ContractListDto>> GetContractsAsync(UserProfileResponseDto user, string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<List<ContractListDto>> GetContractsAsync(UserProfileResponseDto user, string? searchKey);
        Task<bool> SoftDeleteContractAsync(UserProfileResponseDto user, Guid contractId);
        Task<bool> UpdateContractAsync(UserProfileResponseDto user, Guid contractId, ContractUpdateDto newContract);
    }
}
