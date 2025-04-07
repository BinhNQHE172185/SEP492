using LMCM_BE.DTOs.ContractValueItemDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Services.ContractValueItemService
{
    public interface IContractValueItemService
    {
        Task<List<ContractValueItem>> GetListAsync();
        Task<bool> UpdateAsync(List<ContractValueItemDto> newItems);
    }
}
