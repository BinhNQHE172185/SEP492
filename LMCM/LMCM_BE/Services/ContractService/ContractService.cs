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
        public async Task<bool> CreateContract(ContractInsertDto contractDto)
        {
            return await _contractRepository.CreateContract(contractDto);   
        }
        public async Task<ContractDetailDto> GetContractByIdAsync(Guid contractId)
        {
            return await _contractRepository.GetContractByIdAsync(contractId);
        }

        public async Task<PagedResult<ContractListDto>> GetContractsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            return await _contractRepository.GetContractsAsync(searchKey, pageIndex, pageSize); 
        }

        public async Task<List<ContractListDto>> GetContractsAsync(string? searchKey)
        {
            return await _contractRepository.GetContractsAsync(searchKey);
        }

        public async Task<bool> HasActiveConntractsAsync(Guid proposalId)
        {
            return await _contractRepository.HasActiveContractsAsync(proposalId);
        }

        public async Task<bool> SoftDeleteContractAsync(Guid contractId)
        {
            return await _contractRepository.SoftDeleteContractAsync(contractId);
        }

        public async Task<Guid?> UpdateContractAsync(Guid contractId, ContractUpdateDto newContract)
        {
             return await _contractRepository.UpdateContractAsync(contractId, newContract); 
        }
    }
}
