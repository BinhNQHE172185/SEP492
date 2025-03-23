using LMCM_BE.DTOs.BudgetProposalDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.BudgetPropasalRepository;

namespace LMCM_BE.Services.BudgetPropasalService
{
    public class BudgetProposalService : IBudgetProposalService
    {
        private readonly IBudgetProposalRepository _budgetProposalRepository;

        public BudgetProposalService(IBudgetProposalRepository budgetPropasalRepository)
        {
            _budgetProposalRepository = budgetPropasalRepository;
        }
        public async Task<BudgetProposal> CreateBudgetProposal(BudgetProposalInsertDto proposal)
        {
            return await _budgetProposalRepository.CreateBudgetProposal(proposal);  
        }

        public async Task<BudgetProposalDetailDto> GetBudgetProposalById(Guid? proposalId)
        {
            return await _budgetProposalRepository.GetBudgetProposalById(proposalId);
        }

        public async Task<PagedResult<BudgetProposalListDto>> GetBudgetProposalsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            return await _budgetProposalRepository.GetBudgetProposalsAsync(searchKey, pageIndex, pageSize);
        }

        public async Task<Guid?> UpdateBudgetProposalAsync(Guid proposalId, BudgetProposalUpdateDto newProposal)
        {
            return await _budgetProposalRepository.UpdateBudgetProposalAsync(proposalId, newProposal);  
        }
    }
}
