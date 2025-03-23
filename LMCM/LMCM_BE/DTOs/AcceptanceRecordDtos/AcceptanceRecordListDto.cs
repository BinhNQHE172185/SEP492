namespace LMCM_BE.DTOs.AcceptanceRecordDtos
{
    public class AcceptanceRecordListDto
    {
        public Guid AcceptanceId { get; set; }

        public string Title { get; set; } = null!;

        public DateTime? AcceptanceDate { get; set; }

        public decimal? FinalPrice { get; set; }
    }
}
