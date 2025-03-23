using LMCM_BE.DTOs.UserDtos;

namespace LMCM_BE.DTOs.BudgetProposalDtos
{
    public class BudgetProposalDetailDto
    {
        public Guid ProposalId { get; set; }

        public Guid AuthorId { get; set; }

        public string Title { get; set; } = null!;

        public DateTime? ProposalDate { get; set; }

        public string? Url { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ListUserResponseDto Author { get; set; } = null!;
    }
}
