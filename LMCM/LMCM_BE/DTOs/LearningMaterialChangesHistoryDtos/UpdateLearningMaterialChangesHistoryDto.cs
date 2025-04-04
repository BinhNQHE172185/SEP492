namespace LMCM_BE.DTOs.LearningMaterialDtos
{
    public class UpdateLearningMaterialChangesHistoryDto
    {
        public Guid? ContractId { get; set; }
        public Guid SyllabusId { get; set; }
        public string ChangeType { get; set; } = null!;
        public string? ChangeDescription { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string? StartTerm { get; set; }
        public string? CourseCode { get; set; }
    }
}
