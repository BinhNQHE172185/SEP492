using LMCM_BE.DTOs.BudgetProposalDtos;
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
    }
}
