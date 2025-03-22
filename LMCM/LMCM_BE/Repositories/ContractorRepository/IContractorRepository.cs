using LMCM_BE.DTOs.ContractorDtos;
using LMCM_BE.DTOs.ShareDtos;

namespace LMCM_BE.Repositories.ContractorRepository
{
    public interface IContractorRepository
    {
        Task<PagedResult<ContractorListDto>> GetContractorsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<bool> SoftDeleteContractorAsync(Guid contractorId);
        Task<ContractorDetailDto?> GetContractorDetailAsync(Guid contractorId);
        Task<ContractorDetailDto> CreateContractorAsync(ContractorCreateDto dto);
        Task<Guid?> UpdateContractorAsync(Guid contractorId, ContractorUpdateDto dto);
    }
}
