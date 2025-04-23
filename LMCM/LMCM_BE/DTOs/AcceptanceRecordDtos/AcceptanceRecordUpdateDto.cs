using System.ComponentModel.DataAnnotations;
using static LMCM_BE.DTOs.Validators.SharedValidationAtributes;

namespace LMCM_BE.DTOs.AcceptanceRecordDtos
{
    public class AcceptanceRecordUpdateDto
    {
        public Guid ContractId { get; set; }

        [Required(ErrorMessage = "Tên tiêu đề là bắt buộc")]
        [StringLength(100, ErrorMessage = "Độ dài tên tiêu đề không được vượt quá 100 ký tự")]
        [MinLength(3, ErrorMessage = "Phải có ít nhất 3 ký tự")]
        [RegularExpression(@"^\s*\S.*$", ErrorMessage = "Tên tiêu đề không được chỉ chứa khoảng trắng")]
        public string Title { get; set; } = null!;
        [DataType(DataType.Date)]
        public DateTime? AcceptanceDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá trị cuối cùng phải là số không âm")]
        public decimal? FinalPrice { get; set; }

        [AllowedFileExtensions(new string[] { ".pdf" }, ErrorMessage = "Chỉ chấp nhận các tệp có định dạng .pdf")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "Dung lượng tệp không được vượt quá 5MB")]
        public IFormFile? File { get; set; }

    }
}
