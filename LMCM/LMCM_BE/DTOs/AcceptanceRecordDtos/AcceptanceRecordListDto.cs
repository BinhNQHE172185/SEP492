using LMCM_BE.DTOs.ContractDtos;

namespace LMCM_BE.DTOs.AcceptanceRecordDtos
{
    public class AcceptanceRecordListDto
    {
        public Guid AcceptanceId { get; set; }
        public Guid ContractId { get; set; }
        public string Title { get; set; } = null!;

        public DateTime? AcceptanceDate { get; set; }

        public decimal? FinalPrice { get; set; }
        public virtual ContractDetailDto Contract { get; set; } = null!;
    }
}
