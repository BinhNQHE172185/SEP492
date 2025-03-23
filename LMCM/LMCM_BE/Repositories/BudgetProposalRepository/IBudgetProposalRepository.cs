using LMCM_BE.DTOs.BudgetProposalDtos;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Repositories.BudgetPropasalRepository
{
    public interface IBudgetProposalRepository
    {
        Task<BudgetProposal> CreateBudgetProposal(BudgetProposalInsertDto proposal);
        Task<PagedResult<BudgetProposalListDto>> GetBudgetProposalsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<BudgetProposalDetailDto> GetBudgetProposalById(Guid? proposalId);
        Task<Guid?> UpdateBudgetProposalAsync(Guid proposalId, BudgetProposalUpdateDto newProposal);
    }
}
