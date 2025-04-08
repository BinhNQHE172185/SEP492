using LMCM_BE.DbContext;
using LMCM_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.ContractValueItemRepository
{
    public class ContractValueItemRepository : IContractValueItemRepository
    {
        private readonly LMCM_DBContext _dbContext;

        public ContractValueItemRepository(
            LMCM_DBContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        public async Task<List<ContractValueItem>> GetListAsync()
        {
            return await _dbContext.ContractValueItems.ToListAsync();
        }

        public async Task DeleteRangeAsync(List<ContractValueItem> toDelete)
        {
            _dbContext.ContractValueItems.RemoveRange(toDelete);
        }

        public async Task AddRangeAsync(List<ContractValueItem> toAdd)
        {
            await _dbContext.ContractValueItems.AddRangeAsync(toAdd);
        }

        public async Task UpdateRangeAsync(List<ContractValueItem> toUpdate)
        {
            _dbContext.ContractValueItems.UpdateRange(toUpdate);
        }
    }
}
