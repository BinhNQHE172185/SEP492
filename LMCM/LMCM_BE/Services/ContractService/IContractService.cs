using LMCM_BE.DTOs.ContractDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Services.ContractService
{
    public interface IContractService
    {
        Task<bool> CreateContractAsync(ContractInsertDto contractDto);
        Task<ContractDetailDto> GetContractByIdAsync(Guid contractId);
        Task<PagedResult<ContractListDto>> GetContractsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<List<ContractListDto>> GetContractsAsync(string? searchKey);
        Task<bool> SoftDeleteContractAsync(Guid contractId);
        Task<bool> UpdateContractAsync(Guid contractId, ContractUpdateDto newContract);
        Task<Guid?> CheckContractByTitle(string title);
    }
}
