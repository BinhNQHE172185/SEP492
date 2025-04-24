using static LMCM_BE.DTOs.Validators.SharedValidationAtributes;
using System.ComponentModel.DataAnnotations;

namespace LMCM_BE.DTOs.BudgetProposalDtos
{
    public class BudgetProposalUpdateDto
    {
        [Required(ErrorMessage = "Tên tiêu đề là bắt buộc")]
        [StringLength(100, ErrorMessage = "Độ dài tên tiêu đề không được vượt quá 100 ký tự")]
        [MinLength(3, ErrorMessage = "Phải có ít nhất 3 ký tự")]
        [RegularExpression(@"^\s*\S.*$", ErrorMessage = "Tên tiêu đề không được chỉ chứa khoảng trắng")]
        public string Title { get; set; } = null!;

        [DataType(DataType.Date)]
        public DateTime? ProposalDate { get; set; }

        [AllowedFileExtensions(new string[] { ".pdf", ".docx" }, ErrorMessage = "Chỉ chấp nhận các tệp có định dạng .pdf hoặc .docx")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "Dung lượng tệp không được vượt quá 5MB")]
        public IFormFile? File { get; set; }
    }
}
