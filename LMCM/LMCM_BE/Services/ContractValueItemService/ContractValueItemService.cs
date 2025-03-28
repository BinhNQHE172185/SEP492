using LMCM_BE.Models;
using LMCM_BE.Repositories.ContractValueItemRepository;

namespace LMCM_BE.Services.ContractValueItemService
{
    public class ContractValueItemService : IContractValueItemService
    {
        private readonly IContractValueItemRepository _contractValueItemRepository;

        public ContractValueItemService(IContractValueItemRepository contractValueItemRepository)
        {
            _contractValueItemRepository = contractValueItemRepository;
        }

        public async Task<List<ContractValueItem>> GetListAsync()
        {
            return await _contractValueItemRepository.GetListAsync();
        }

        public async Task UpdateAsync(List<ContractValueItem> newItems)
        {
            await _contractValueItemRepository.UpdateAsync(newItems);
        }
    }
}
