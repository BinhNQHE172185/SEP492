namespace LMCM_BE.DTOs.BudgetProposalDtos
{
    public class BudgetProposalInsertDto
    {
        public string Title { get; set; } = null!;

        public DateTime? ProposalDate { get; set; }

        public IFormFile? File { get; set; }
    }
}
