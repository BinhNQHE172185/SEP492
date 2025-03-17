namespace LMCM_BE.DTOs.LearningMaterialDtos
{
    public class LearningMaterialViewDto
    {
        public Guid SyllabusId { get; set; }
        public Guid MaterialId { get; set; }
        public Guid? MaterialDetailId { get; set; }

        public string? LearningType { get; set; }
        public string? MaterialType { get; set; }

        public bool? IsMainMaterial { get; set; }

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
