namespace LMCM_BE.DTOs.LearningMaterialDtos
{
    public class LearningMaterialDetailsInsertDto
    {
        public Guid MaterialId { get; set; }

        public Guid SyllabusId { get; set; }

        public Guid MaterialDetailId { get; set; }

        public string? Type { get; set; }

        public bool? IsMainMaterial { get; set; }

        public int? MaterialNo { get; set; }

        public string? MaterialName { get; set; }

        public string? MaterialQuantity { get; set; }

        public string? Url { get; set; }

        public string? Purpose { get; set; }

        public string? Note { get; set; }
    }
}
