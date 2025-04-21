using LMCM_BE.Models;

namespace LMCM_BE.Repositories.ContractRepository
{
    public interface IContractRepository
    {
        Task<bool> CreateContractAsync(Contract contract);
        Task<Contract> GetContractDetailByIdAsync(Guid contractId);
        Task<Contract?> GetContractByIdAsync(Guid contractId);
        Task<Contract?> GetActiveContractByIdAsync(Guid contractId);
        Task<(List<Contract>,int totalCount)> GetContractsAsync(bool isHod, Guid userId,string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<List<Contract>> GetContractsAsync(bool isHod, Guid userId, string? searchKey);
        Task<bool> HasActiveContractsAsync(Guid contractorId);
        Task<bool> HasActiveConntractsAsync(Guid proposalId);
        Task<bool> UpdateContractAsync(Contract newContract);
        Task<Guid?> CheckContractByTitle(string title);
        Task<Guid?> GetDuplicatedTitleIdAsync(string title);
    }
}
