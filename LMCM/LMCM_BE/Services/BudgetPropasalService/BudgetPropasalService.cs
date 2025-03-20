using LMCM_BE.DTOs.BudgetProposalDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.BudgetPropasalRepository;

namespace LMCM_BE.Services.BudgetPropasalService
{
    public class BudgetPropasalService : IBudgetPropasalService
    {
        private readonly IBudgetPropasalRepository _budgetPropasalRepository;

        public BudgetPropasalService(IBudgetPropasalRepository budgetPropasalRepository)
        {
            _budgetPropasalRepository = budgetPropasalRepository;
        }
        public async Task<BudgetProposal> CreateBudgetPropasal(BudgetProposalInsertDto propasal)
        {
            return await _budgetPropasalRepository.CreateBudgetPropasal(propasal);  
        }

        public async Task<BudgetPropasalDetailDto> GetBudgetPropasalById(Guid? propasalId)
        {
            return await _budgetPropasalRepository.GetBudgetPropasalById(propasalId);
        }

        public async Task<PagedResult<BudgetProposalListDto>> GetBudgetPropasalsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            return await _budgetPropasalRepository.GetBudgetPropasalsAsync(searchKey, pageIndex, pageSize);
        }
    }
}
