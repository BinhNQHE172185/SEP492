using LMCM_BE.Models;

namespace LMCM_BE.DTOs.LearningMaterialDtos
{
    public class ChangesHistoryListDto
    {
        public Guid HistoryId { get; set; }

        public Guid UserId { get; set; }

        public Guid? ContractId { get; set; }

        public Guid NewMaterialId { get; set; }

        public Guid? OldMaterialId { get; set; }

        public string LearningMaterialType { get; set; } = null!;

        public string ChangeType { get; set; } = null!;

        public DateTime? CompletionDate { get; set; }

        public string? StartTerm { get; set; }

        public string? CourseCode { get; set; }

        public string? Status { get; set; }
    }
}
