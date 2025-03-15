using LMCM_BE.DTOs.ContractDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.ContractRepository;
using LMCM_BE.Repositories.CurriculumRepository;

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
    }
}
