using System.ComponentModel.DataAnnotations;

namespace LMCM_BE.DTOs.ContractorDtos
{
    public class ContractorCreateDto
    {
        [Required(ErrorMessage = "Tên nhà thầu là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên nhà thầu không được vượt quá 100 ký tự")]
        [MinLength(3, ErrorMessage = "Tên nhà thầu phải có ít nhất 3 ký tự")]
        [RegularExpression(@"^\s*\S.*$", ErrorMessage = "Tên nhà thầu không được chỉ chứa khoảng trắng")]
        public string ContractorName { get; set; } = null!;

        public string? Address { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string? PhoneNumber { get; set; }

        public string? TaxCode { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string? Email { get; set; }

        public string? EmployeeCode { get; set; }

        public string? IdCardNumber { get; set; }

        public string? IdIssuedPlace { get; set; }

        public string? Position { get; set; }

        public string? BankAccountNumber { get; set; }

        public string? BankName { get; set; }
    }
}
