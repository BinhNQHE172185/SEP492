using LMCM_BE.Models;

namespace LMCM_BE.DTOs.LearningMaterialDtos
{
    public class LearningMaterialListDto
    {
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

        public virtual LearningMaterialDetailDto? MaterialDetail { get; set; }
    }
}
