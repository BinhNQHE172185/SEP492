using LMCM_BE.Models;

namespace LMCM_BE.DTOs.ContractDtos
{
    public class ContractUpdateDto
    {
        public Guid ContractorId { get; set; }

        public string Title { get; set; } = null!;

        public DateTime? ContractDate { get; set; }

        public decimal ContractValue { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public IFormFile? File { get; set; }
    }
}
