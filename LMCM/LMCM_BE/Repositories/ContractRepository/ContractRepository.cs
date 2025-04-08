using LMCM_BE.DbContext;
using LMCM_BE.Models;
using LMCM_BE.Shared.Constant;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.ContractRepository
{
    public class ContractRepository : IContractRepository
    {
        private readonly LMCM_DBContext _dbContext;

        public ContractRepository(LMCM_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CreateContract(Contract contract)
        {
            await _dbContext.Contracts.AddAsync(contract);

            return true;
        }

        public async Task<List<Contract>> GetContractsAsync(bool isHod, Guid userId, string? searchKey)
        {
            var query = _dbContext.Contracts.AsQueryable();

            query = query.OrderByDescending(s => s.UpdatedAt);

            if (!isHod) query = query.Where(s => s.AuthorId == userId);

            query = query.Where(s => s.Status == GenericStatus.Active);

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                query = query.Where(s => s.Author.Email.ToLower().Contains(search) ||
                                         s.Title.ToLower().Contains(search) ||
                                         s.Author.UserName.ToLower().Contains(search));
            }

            var items = await query
                .Include(s => s.Author)
                .Include(s => s.Proposal)
                .Include(s => s.Contractor)
                .ToListAsync();

            return items;
        }

        public async Task<Contract> GetContractDetailByIdAsync(Guid contractId)
        {
            var contract = await _dbContext.Contracts
                .Where(s => s.ContractId == contractId)
                .Include(s => s.Author)
                .Include(s => s.Proposal)
                .Include(s => s.Contractor)
                .SingleOrDefaultAsync();

            return contract;
        }
        public async Task<Contract?> GetActiveContractByIdAsync(Guid contractId)
        {
            return await _dbContext.Contracts
                .Include(s => s.Author)
                .FirstOrDefaultAsync(s => s.ContractId == contractId && s.Status == GenericStatus.Active);
        }
        public async Task<Contract?> GetContractByIdAsync(Guid contractId)
        {
            return await _dbContext.Contracts
                .Include(s => s.Author)
                .FirstOrDefaultAsync(s => s.ContractId == contractId);
        }

        public async Task<(List<Contract>, int totalCount)> GetContractsAsync(bool isHod, Guid userId, string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.Contracts.AsQueryable();

            query = query.OrderByDescending(s => s.UpdatedAt);

            if (!isHod) query = query.Where(s => s.AuthorId == userId);

            query = query.Where(s => s.Status == GenericStatus.Active);

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                query = query.Where(s => s.Author.Email.ToLower().Contains(search) ||
                                         s.Title.ToLower().Contains(search) ||
                                         s.Author.UserName.ToLower().Contains(search));
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Include(s => s.Author)
                .Include(s => s.Proposal)
                .Include(s => s.Contractor)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<bool> HasActiveConntractsAsync(Guid proposalId)
        {
            return await _dbContext.Contracts
                  .AnyAsync(p => p.ProposalId == proposalId && p.Status == GenericStatus.Active);
        }

        public async Task<bool> HasActiveContractsAsync(Guid contractorId)
        {
            return await _dbContext.Contracts
                .AnyAsync(p => p.ContractorId == contractorId && p.Status == GenericStatus.Active);
        }

        public async Task<bool> UpdateContractAsync(Contract newContract)
        {
            _dbContext.Contracts.Update(newContract);
            return true;
        }

    }
}
