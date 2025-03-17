namespace LMCM_BE.DTOs.BudgetProposalDtos
{
    public class BudgetProposalInsertDto
    {
        public Guid AuthorId { get; set; }

        public string Title { get; set; } = null!;

        public DateTime? ProposalDate { get; set; }

        public IFormFile? File { get; set; }
    }
}
