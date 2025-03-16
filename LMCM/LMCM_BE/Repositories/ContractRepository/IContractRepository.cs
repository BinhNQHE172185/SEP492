using LMCM_BE.DTOs.ContractDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Repositories.ContractRepository
{
    public interface IContractRepository
    {
        Task<Contract> CreateContract(ContractInsertDto contract);
        Task<Contract?> GetContractByIdAsync(Guid contractId);
    }
}
