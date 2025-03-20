using LMCM_BE.DTOs.ContractDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.ContractRepository;

namespace LMCM_BE.Services.ContractService
{
    public class ContractService : IContractService
    {
        private readonly IContractRepository _contractRepository;

        public ContractService(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }
        public async Task<Contract> CreateContract(ContractInsertDto contractDto)
        {
            return await _contractRepository.CreateContract(contractDto);   
        }
        public async Task<Contract?> GetContractByIdAsync(Guid contractId)
        {
            return await _contractRepository.GetContractByIdAsync(contractId);
        }

        public async Task<PagedResult<ContractListDto>> GetContractsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            return await _contractRepository.GetContractsAsync(searchKey, pageIndex, pageSize); 
        }
    }
}
