using System.ComponentModel.DataAnnotations;
using static LMCM_BE.DTOs.Validators.SharedValidationAtributes;

namespace LMCM_BE.DTOs.DocumentTemplateDtos
{
    public class DocumentTemplateInsertDto
    {
        [StringLength(100, ErrorMessage = "Loại mẫu tài liệu không được vượt quá 100 ký tự")]
        public string? TemplateType { get; set; }

        [Required(ErrorMessage = "Tên mẫu tài liệu là bắt buộc")]
        [StringLength(100, ErrorMessage = "Độ dài tên không được vượt quá 100 ký tự")]
        [MinLength(3, ErrorMessage = "Tên phải có ít nhất 3 ký tự")]
        [RegularExpression(@"^\s*\S.*$", ErrorMessage = "Tên không được chỉ chứa khoảng trắng")]
        public string TemplateName { get; set; } = null!;

        [Required(ErrorMessage = "File đính kèm là bắt buộc")]
        [AllowedFileExtensions(new string[] { ".pdf", ".docx" }, ErrorMessage = "Chỉ chấp nhận các tệp có định dạng .pdf hoặc .docx")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "Dung lượng tệp không được vượt quá 5MB")]
        public IFormFile? File { get; set; }
    }
}
