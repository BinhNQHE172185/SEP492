using LMCM_BE.Models;
using LMCM_BE.Models.Constant;

namespace LMCM_BE.DTOs.LearningMaterialDtos
{
    public class ChangesHistoryListDto
    {
        public Guid HistoryId { get; set; }

        public Guid UserId { get; set; }

        public Guid? ContractId { get; set; }

        public Guid SyllabusId { get; set; }

        public string ChangeType { get; set; } = null!;

        public DateTime? CompletionDate { get; set; }

        public string? StartTerm { get; set; }

        public string? CourseCode { get; set; }

        public GenericStatus Status { get; set; }
    }
}
