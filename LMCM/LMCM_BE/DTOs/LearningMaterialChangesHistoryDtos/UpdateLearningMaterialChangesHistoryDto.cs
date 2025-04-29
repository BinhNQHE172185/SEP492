using System.ComponentModel.DataAnnotations;

namespace LMCM_BE.DTOs.LearningMaterialDtos
{
    public class UpdateLearningMaterialChangesHistoryDto
    {
        public Guid? ContractId { get; set; }
        [Required(ErrorMessage = "SyllabusId là bắt buộc.")]
        public Guid SyllabusId { get; set; }
        [Required(ErrorMessage = "Loại thay đổi là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Loại thay đổi không được vượt quá 100 ký tự.")]
        [MinLength(3, ErrorMessage = "Loại thay đổi phải có ít nhất 3 ký tự.")]
        [RegularExpression(@"^\s*\S.*$", ErrorMessage = "Loại thay đổi không được chỉ chứa khoảng trắng.")]
        public string ChangeType { get; set; } = null!;
        [StringLength(200, ErrorMessage = "Loại thay đổi không được vượt quá 200 ký tự.")]
        public string? ChangeDescription { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CompletionDate { get; set; }
        public string? StartTerm { get; set; }
        public string? CourseCode { get; set; }
    }
}
