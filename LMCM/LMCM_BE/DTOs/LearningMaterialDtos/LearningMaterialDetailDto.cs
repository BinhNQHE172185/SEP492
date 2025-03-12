namespace LMCM_BE.DTOs.LearningMaterialDtos
{
    public class LearningMaterialDetailDto
    {
        public Guid MaterialDetailId { get; set; }

        public string? MaterialName { get; set; }

        public string? MaterialDescription { get; set; }

        public string? Isbn { get; set; }

        public string? Type { get; set; }

        public string? Note { get; set; }

        public string? Author { get; set; }

        public string? Publisher { get; set; }

        public DateTime? PublishedDate { get; set; }

        public string? Edition { get; set; }

        public string? Url { get; set; }
    }
}
