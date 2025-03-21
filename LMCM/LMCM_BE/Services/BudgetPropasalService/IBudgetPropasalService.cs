using LMCM_BE.DTOs.BudgetProposalDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Services.BudgetPropasalService
{
    public interface IBudgetPropasalService
    {
        Task<BudgetProposal> CreateBudgetPropasal(BudgetProposalInsertDto propasal);
        Task<PagedResult<BudgetProposalListDto>> GetBudgetPropasalsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<BudgetPropasalDetailDto> GetBudgetPropasalById(Guid? propasalId);
        Task<Guid?> UpdateBudgetPropasalAsync(Guid propasalId, BudgetProposalUpdateDto newPropasal);
    }
}
