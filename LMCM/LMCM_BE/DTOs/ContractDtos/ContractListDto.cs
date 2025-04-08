using LMCM_BE.DTOs.BudgetProposalDtos;
using LMCM_BE.DTOs.ContractorDtos;
using LMCM_BE.DTOs.UserDtos;

namespace LMCM_BE.DTOs.ContractDtos
{
    public class ContractListDto
    {
        public Guid ContractId { get; set; }
        public Guid ProposalId { get; set; }

        public Guid AuthorId { get; set; }

        public Guid ContractorId { get; set; }

        public string Title { get; set; } = null!;

        public DateTime? ContractDate { get; set; }

        public decimal ContractValue { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? Url { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ListUserResponseDto Author { get; set; } = null!;

        public virtual ContractorDetailDto Contractor { get; set; } = null!; 

        //public virtual ICollection<LearningMaterialChangesHistory> LearningMaterialChangesHistories { get; set; } = new List<LearningMaterialChangesHistory>(); todo

        public virtual BudgetProposalDetailDto Proposal { get; set; } = null!;
    }
}
