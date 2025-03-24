using LMCM_BE.DTOs.BudgetProposalDtos;
using LMCM_BE.DTOs.ContractDtos;
using LMCM_BE.DTOs.ContractorDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Repositories.ContractRepository
{
    public interface IContractRepository
    {
        Task<bool> CreateContract(ContractInsertDto contract);
        Task<ContractDetailDto> GetContractByIdAsync(Guid contractId);
        Task<PagedResult<ContractListDto>> GetContractsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<bool> HasActiveContractsAsync(Guid contractorId);
        Task<bool> HasActiveConntractsAsync(Guid proposalId);
        Task<bool> SoftDeleteContractAsync(Guid contractId, Guid authorId);
        Task<Guid?> UpdateContractAsync(Guid contractId, ContractUpdateDto newContract);
    }
}
