using LMCM_BE.DTOs.BudgetProposalDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Services.BudgetPropasalService
{
    public interface IBudgetProposalService
    {
        Task<bool> CreateBudgetProposalAsync(BudgetProposalInsertDto proposal);
        Task<PagedResult<BudgetProposalListDto>> GetBudgetProposalsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<BudgetProposalDetailDto> GetBudgetProposalByIdAsync(Guid proposalId);
        Task<bool> SoftDeleteBudgetProposalAsync(Guid proposalId, Guid authorId);
        Task<Guid?> UpdateBudgetProposalAsync(Guid propasalId, BudgetProposalUpdateDto newProposal);
    }
}
