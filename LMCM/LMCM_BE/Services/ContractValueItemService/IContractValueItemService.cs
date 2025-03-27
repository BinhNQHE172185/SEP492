using LMCM_BE.Models;

namespace LMCM_BE.Services.ContractValueItemService
{
    public interface IContractValueItemService
    {
        Task<List<ContractValueItem>> GetListAsync();
        Task UpdateAsync(List<ContractValueItem> newItems);
    }
}
