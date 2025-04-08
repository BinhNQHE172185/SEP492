using LMCM_BE.Models.Constant;

namespace LMCM_BE.DTOs.ContractorDtos
{
    public class ContractorListDto
    {
        public Guid ContractorId { get; set; }

        public string ContractorName { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? Position { get; set; }

        public GenericStatus Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
