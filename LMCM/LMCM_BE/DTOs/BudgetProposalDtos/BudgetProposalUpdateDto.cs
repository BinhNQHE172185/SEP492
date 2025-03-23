using LMCM_BE.DTOs.UserDtos;

namespace LMCM_BE.DTOs.BudgetProposalDtos
{
    public class BudgetProposalUpdateDto
    {
        public Guid AuthorId { get; set; }

        public string Title { get; set; } = null!;

        public DateTime? ProposalDate { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public IFormFile? File { get; set; }
    }
}
