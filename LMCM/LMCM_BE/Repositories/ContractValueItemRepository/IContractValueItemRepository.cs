using LMCM_BE.Models;

namespace LMCM_BE.Repositories.ContractValueItemRepository
{
    public interface IContractValueItemRepository
    {
        Task<List<ContractValueItem>> GetListAsync();
        Task UpdateAsync(List<ContractValueItem> newItems);
    }
}
