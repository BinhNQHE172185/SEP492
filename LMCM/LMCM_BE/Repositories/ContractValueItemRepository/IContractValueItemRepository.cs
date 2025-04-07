using LMCM_BE.Models;

namespace LMCM_BE.Repositories.ContractValueItemRepository
{
    public interface IContractValueItemRepository
    {
        Task<List<ContractValueItem>> GetListAsync();
        Task DeleteRangeAsync(List<ContractValueItem> toDelete);
        Task AddRangeAsync(List<ContractValueItem> toAdd);
        Task UpdateRangeAsync(List<ContractValueItem> toUpdate);
    }
}
