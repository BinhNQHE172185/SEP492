using LMCM_BE.DTOs.UserDtos;

namespace LMCM_BE.DTOs.BudgetProposalDtos
{
    public class BudgetProposalUpdateDto
    {
        public string Title { get; set; } = null!;

        public DateTime? ProposalDate { get; set; }
        public IFormFile? File { get; set; }
    }
}
