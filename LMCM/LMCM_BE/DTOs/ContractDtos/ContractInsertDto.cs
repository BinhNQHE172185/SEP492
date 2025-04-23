using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using static LMCM_BE.DTOs.Validators.SharedValidationAtributes;

namespace LMCM_BE.DTOs.ContractDtos
{
    public class ContractInsertDto
    {
        [Required(ErrorMessage = "ProposalId là bắt buộc")]
        public Guid ProposalId { get; set; }

        [Required(ErrorMessage = "ContractorId là bắt buộc")]
        public Guid ContractorId { get; set; }

        [Required(ErrorMessage = "Tiêu đề hợp đồng là bắt buộc")]
        [StringLength(100, ErrorMessage = "Độ dài tiêu đề không được vượt quá 100 ký tự")]
        [MinLength(3, ErrorMessage = "Tiêu đề phải có ít nhất 3 ký tự")]
        [RegularExpression(@"^\s*\S.*$", ErrorMessage = "Tiêu đề không được chỉ chứa khoảng trắng")]
        public string Title { get; set; } = null!;

        [DataType(DataType.Date)]
        public DateTime? ContractDate { get; set; }

        [Required(ErrorMessage = "Giá trị hợp đồng là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá trị hợp đồng phải lớn hơn hoặc bằng 0")]
        public decimal ContractValue { get; set; }
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "File đính kèm là bắt buộc")]
        [AllowedFileExtensions(new string[] { ".pdf", ".docx" }, ErrorMessage = "Chỉ chấp nhận các tệp có định dạng .pdf hoặc .docx")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "Dung lượng tệp không được vượt quá 5MB")]
        public IFormFile? File { get; set; }
    }
}
