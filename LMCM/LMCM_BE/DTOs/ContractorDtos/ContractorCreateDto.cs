using System.ComponentModel.DataAnnotations;

namespace LMCM_BE.DTOs.ContractorDtos
{
    public class ContractorCreateDto
    {
        [Required(ErrorMessage = "Tên chuyên gia là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên chuyên gia không được vượt quá 100 ký tự")]
        [MinLength(3, ErrorMessage = "Tên chuyên gia phải có ít nhất 3 ký tự")]
        [RegularExpression(@"^\s*\S.*$", ErrorMessage = "Tên chuyên gia không được chỉ chứa khoảng trắng")]
        public string ContractorName { get; set; } = null!;

        public string? Address { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Mã số thuế là bắt buộc")]
        [StringLength(20, ErrorMessage = "Mã số thuế không được vượt quá 20 ký tự")]
        public string? TaxCode { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Mã nhân viên là bắt buộc")]
        public string? EmployeeCode { get; set; }

        [Required(ErrorMessage = "Số CMND là bắt buộc")]
        public string? IdCardNumber { get; set; }

        public string? IdIssuedPlace { get; set; }

        public string? Position { get; set; }

        public string? BankAccountNumber { get; set; }

        public string? BankName { get; set; }
    }
}
