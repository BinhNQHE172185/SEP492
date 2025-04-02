namespace LMCM_BE.DTOs.LearningMaterialDtos
{
    public class LearningMaterialViewDto
    {
        public string? LearningType { get; set; }
        public bool? IsMainMaterial { get; set; }
        public string? Purpose { get; set; }

        public int? MaterialNo { get; set; }

        public string? MaterialName { get; set; }

        public string? MaterialQuantity { get; set; }

        public string? Url { get; set; }

        public string? Note { get; set; }
        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public virtual LearningMaterialDetailDto? MaterialDetail { get; set; }
    }
}
