using LMCM_BE.DTOs.BudgetProposalDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Services.BudgetPropasalService
{
    public interface IBudgetPropasalService
    {
        Task<BudgetProposal> CreateBudgetPropasal(BudgetProposalInsertDto propasal);
    }
}
