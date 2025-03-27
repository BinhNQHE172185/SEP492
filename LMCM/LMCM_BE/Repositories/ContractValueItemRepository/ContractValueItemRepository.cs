using AutoMapper;
using LMCM_BE.DbContext;
using LMCM_BE.Models;
using LMCM_BE.Repositories.ContractRepository;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.ContractValueItemRepository
{
    public class ContractValueItemRepository : IContractValueItemRepository
    {
        private readonly LMCM_DBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IContractRepository _contractRepository;

        public ContractValueItemRepository(
            LMCM_DBContext dbContext,
            IMapper mapper,
            IContractRepository contractRepository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _contractRepository = contractRepository;
        }

        public async Task<List<ContractValueItem>> GetListAsync()
        {
            return await _dbContext.ContractValueItems.ToListAsync();
        }

        public async Task UpdateAsync(List<ContractValueItem> newItems)
        {
            if (newItems == null)
            {
                throw new ArgumentException("Danh sách không được để trống.");
            }

            var existingItems = await _dbContext.ContractValueItems.ToListAsync();

            // Find items to delete
            var toDelete = existingItems.Where(e => !newItems.Any(n => n.ValueId == e.ValueId)).ToList();
            _dbContext.ContractValueItems.RemoveRange(toDelete);

            // Find items to update or add
            foreach (var newItem in newItems)
            {
                var existingItem = existingItems.FirstOrDefault(e => e.ValueId == newItem.ValueId);
                if (existingItem != null)
                {
                    _mapper.Map(newItem, existingItem);
                    _dbContext.ContractValueItems.Update(existingItem);
                }
                else
                {
                    await _dbContext.ContractValueItems.AddAsync(newItem);
                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
