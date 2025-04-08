using LMCM_BE.Models.Constant;

namespace LMCM_BE.DTOs.LearningMaterialDtos
{
    public class LearningMaterialViewDto
    {
        public string? LearningType { get; set; }
        public bool? IsImportedMaterial { get; set; }
        public MaterialType MaterialType { get; set; }

        public bool? IsMainMaterial { get; set; }

        public string? MaterialName { get; set; }

        public string? Isbn { get; set; }


        public string? Author { get; set; }

        public string? Publisher { get; set; }

        public DateTime? PublishedDate { get; set; }

        public string? Edition { get; set; }

        public string? Url { get; set; }

        public string? Purpose { get; set; }

        public string? Note { get; set; }

        public LearningMaterialStatus Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
