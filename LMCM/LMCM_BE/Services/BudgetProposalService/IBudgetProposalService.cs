using LMCM_BE.DTOs.BudgetProposalDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.UserDtos;

namespace LMCM_BE.Services.BudgetPropasalService
{
    public interface IBudgetProposalService
    {
        Task<bool> CreateBudgetProposalAsync(UserProfileResponseDto user, BudgetProposalInsertDto proposal);
        Task<PagedResult<BudgetProposalListDto>> GetBudgetProposalsAsync(UserProfileResponseDto user, string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<List<BudgetProposalListDto>> GetBudgetProposalsAsync(UserProfileResponseDto user, string? searchKey);
        Task<BudgetProposalDetailDto> GetBudgetProposalByIdAsync(UserProfileResponseDto user, Guid proposalId);
        Task<bool> SoftDeleteBudgetProposalAsync(UserProfileResponseDto user, Guid proposalId);
        Task<bool> UpdateBudgetProposalAsync(UserProfileResponseDto user, Guid propasalId, BudgetProposalUpdateDto newProposal);
    }
}
