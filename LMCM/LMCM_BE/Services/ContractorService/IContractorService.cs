using LMCM_BE.DTOs.ContractorDtos;
using LMCM_BE.DTOs.ShareDtos;

namespace LMCM_BE.Services.ContractorService
{
    public interface IContractorService
    {
        Task<PagedResult<ContractorListDto>> GetContractorsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<List<ContractorListDto>> GetContractorsListAsync();
        Task<bool> SoftDeleteContractorAsync(Guid contractorId);
        Task<ContractorDetailDto?> GetContractorDetailAsync(Guid contractorId);
        Task<ContractorDetailDto> CreateContractorAsync(ContractorCreateDto dto);
        Task<Guid?> UpdateContractorAsync(Guid contractorId, ContractorUpdateDto dto);
    }
}
