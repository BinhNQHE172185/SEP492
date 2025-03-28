namespace LMCM_BE.DTOs.AcceptanceRecordDtos
{
    public class AcceptanceRecordUpdateDto
    {
        public Guid ContractId { get; set; }

        public string Title { get; set; } = null!;

        public DateTime? AcceptanceDate { get; set; }

        public decimal? FinalPrice { get; set; }

        public IFormFile? File { get; set; }

        public string? Status { get; set; }
    }
}
