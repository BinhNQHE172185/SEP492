using LMCM_BE.Models;

namespace LMCM_BE.DTOs.LearningMaterialDtos
{
    public class LearningMaterialInsertDto
    {
        public Guid SyllabusId { get; set; }

        public string? LearningType { get; set; }
        public string? MaterialType { get; set; }
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
    }
}
