using LMCM_BE.Models;
using LMCM_BE.Shared.Constant;

namespace LMCM_BE.DTOs.LearningMaterialChangesHistoryDtos
{
    public class ChangesHistoryDetailDto
    {
        public Guid HistoryId { get; set; }

        public Guid UserId { get; set; }

        public string? Username { get; set; }

        public Guid? ContractId { get; set; }

        public string? ContractTitle { get; set; }

        public Guid SyllabusId { get; set; }

        public string ChangeType { get; set; } = null!;

        public string? ChangeDescription { get; set; }

        public DateTime? CompletionDate { get; set; }

        public string? StartTerm { get; set; }

        public string? CourseCode { get; set; }

        public GenericStatus Status { get; set; }
    }
}
