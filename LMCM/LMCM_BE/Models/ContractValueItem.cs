namespace LMCM_BE.Models
{
    public class ContractValueItem
    {
        public Guid ValueId { get; set; } = Guid.NewGuid();
        public string Category { get; set; } = null!;
        public string MeasurementUnit { get; set; } = null!;
        public decimal StandardRate { get; set; }
        public string? QualityRequirements { get; set; } 
        public decimal ValueRatioForUpdate { get; set; } = 0.5m; 
        public decimal ContractValue { get; set; }
    }
}
