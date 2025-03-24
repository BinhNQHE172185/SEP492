using LMCM_BE.DTOs.ContractDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Services.ContractService
{
    public interface IContractService
    {
        Task<Contract> CreateContract(ContractInsertDto contractDto);
        Task<ContractDetailDto> GetContractByIdAsync(Guid contractId);
        Task<PagedResult<ContractListDto>> GetContractsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<bool> SoftDeleteContractAsync(Guid contractId, Guid authorId);
        Task<bool> HasActiveConntractsAsync(Guid proposalId);
        Task<Guid?> UpdateContractAsync(Guid contractId, ContractUpdateDto newContract);
    }
}
