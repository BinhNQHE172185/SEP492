using LMCM_BE.DTOs.ContractDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Services.ContractService
{
    public interface IContractService
    {
        Task<Contract> CreateContract(ContractInsertDto contractDto);
        Task<Contract?> GetContractByIdAsync(Guid contractId);
    }
}
