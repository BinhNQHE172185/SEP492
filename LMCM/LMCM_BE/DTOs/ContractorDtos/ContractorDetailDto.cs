using LMCM_BE.Models.Constant;

namespace LMCM_BE.DTOs.ContractorDtos
{
    public class ContractorDetailDto
    {
        public Guid ContractorId { get; set; }

        public string ContractorName { get; set; } = null!;

        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }

        public string? TaxCode { get; set; }

        public string? Email { get; set; }

        public string? EmployeeCode { get; set; }

        public string? IdCardNumber { get; set; }

        public string? IdIssuedPlace { get; set; }

        public string? Position { get; set; }

        public string? BankAccountNumber { get; set; }

        public string? BankName { get; set; }

        public GenericStatus Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
