namespace LMCM_BE.DTOs.LearningMaterialDtos
{
    public class CreateLearningMaterialChangesHistoryDto
    {
        public Guid? ContractId { get; set; }

        public Guid SyllabusId { get; set; }

        public string LearningMaterialType { get; set; } = null!;

        public string ChangeType { get; set; } = null!;

        public string? ChangeDescription { get; set; }

        public DateTime? CompletionDate { get; set; }

        public string? StartTerm { get; set; }

        public string? CourseCode { get; set; }
    }
}
