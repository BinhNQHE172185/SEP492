using LMCM_BE.DTOs.ContractDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Services.ContractService
{
    public interface IContractService
    {
        Task<Contract> CreateContract(ContractInsertDto contractDto);
        Task<Contract?> GetContractByIdAsync(Guid contractId);
        Task<PagedResult<ContractListDto>> GetContractsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
    }
}
