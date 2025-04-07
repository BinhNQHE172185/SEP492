using LMCM_BE.Models;

namespace LMCM_BE.Repositories.BudgetPropasalRepository
{
    public interface IBudgetProposalRepository
    {
        Task<bool> CreateBudgetProposalAsync(BudgetProposal proposal);
        Task<(List<BudgetProposal>,int totalCount)> GetBudgetProposalsAsync(bool isHod,Guid userId, string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<List<BudgetProposal>> GetBudgetProposalsAsync(bool isHod, Guid userId,string? searchKey);
        Task<BudgetProposal> GetBudgetProposalByIdAsync(Guid proposalId);
        Task<BudgetProposal> GetActiveBudgetProposalByIdAsync(Guid proposalId);

        Task<bool> SoftDeleteBudgetProposalAsync(BudgetProposal budgetProposal);
        Task<bool> UpdateBudgetProposalAsync(BudgetProposal newProposal);
    }
}
