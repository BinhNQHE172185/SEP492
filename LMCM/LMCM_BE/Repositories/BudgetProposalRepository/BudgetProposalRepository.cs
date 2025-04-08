using LMCM_BE.DbContext;
using LMCM_BE.Models;
using LMCM_BE.Models.Constant;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.BudgetPropasalRepository
{
    public class BudgetProposalRepository : IBudgetProposalRepository
    {
        private readonly LMCM_DBContext _dbContext;

        public BudgetProposalRepository(LMCM_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> CreateBudgetProposalAsync(BudgetProposal proposal)
        {
            await _dbContext.BudgetProposals.AddAsync(proposal);

            return true;
        }
        public async Task<BudgetProposal> GetBudgetProposalByIdAsync(Guid proposalId)
        {
            var budgetProposal = await _dbContext.BudgetProposals
                .Include(s => s.Author)  
                .Where(s => s.ProposalId == proposalId)
                .SingleOrDefaultAsync();

            return budgetProposal;
        }

        public async Task<BudgetProposal> GetActiveBudgetProposalByIdAsync(Guid proposalId)
        {
            var budgetProposal = await _dbContext.BudgetProposals
                .Include(s => s.Author)
                .Where(s => s.ProposalId == proposalId && s.Status == GenericStatus.Active)
                .SingleOrDefaultAsync();

            return budgetProposal;
        }
        public async Task<(List<BudgetProposal>, int totalCount)> GetBudgetProposalsAsync(bool isHod, Guid userId, string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.BudgetProposals.AsQueryable();

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
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<BudgetProposal>> GetBudgetProposalsAsync(bool isHod, Guid userId, string? searchKey)
        {
            var query = _dbContext.BudgetProposals.AsQueryable();

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
                .ToListAsync();

            return items;
        }

        public async Task<bool> UpdateBudgetProposalAsync(BudgetProposal newProposal)
        {
            _dbContext.Update(newProposal);
            return true;
        }
        public async Task<int> BudgetCountAsync()
        {
            var budgetCount = await _dbContext.BudgetProposals
                .Where(s => s.Status == GenericStatus.Active)
                .CountAsync();
            return budgetCount;
        }
    }
}
