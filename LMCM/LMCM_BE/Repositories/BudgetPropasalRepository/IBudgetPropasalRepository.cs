using LMCM_BE.DTOs.BudgetProposalDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Repositories.BudgetPropasalRepository
{
    public interface IBudgetPropasalRepository
    {
        Task<BudgetProposal> CreateBudgetPropasal(BudgetProposalInsertDto propasal);
    }
}
