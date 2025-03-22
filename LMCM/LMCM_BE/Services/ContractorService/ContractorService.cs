using LMCM_BE.DTOs.ContractorDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.ContractorRepository;

namespace LMCM_BE.Services.ContractorService
{
    public class ContractorService : IContractorService
    {
        private readonly IContractorRepository _contractorRepository;

        public ContractorService(IContractorRepository contractorRepository)
        {
            _contractorRepository = contractorRepository;
        }

        public async Task<PagedResult<ContractorListDto>> GetContractorsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            return await _contractorRepository.GetContractorsAsync(searchKey, pageIndex, pageSize);
        }

        public async Task<bool> SoftDeleteContractorAsync(Guid contractorId)
        {
            return await _contractorRepository.SoftDeleteContractorAsync(contractorId);
        }

        public async Task<ContractorDetailDto?> GetContractorDetailAsync(Guid contractorId)
        {
            return await _contractorRepository.GetContractorDetailAsync(contractorId);
        }

        public async Task<ContractorDetailDto> CreateContractorAsync(ContractorCreateDto dto)
        {
            return await _contractorRepository.CreateContractorAsync(dto);
        }

        public async Task<Guid?> UpdateContractorAsync(Guid contractorId, ContractorUpdateDto dto)
        {
            return await _contractorRepository.UpdateContractorAsync(contractorId, dto);
        }
    }
}
