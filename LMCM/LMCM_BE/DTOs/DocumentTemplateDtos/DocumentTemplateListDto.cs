using LMCM_BE.DTOs.UserDtos;

namespace LMCM_BE.DTOs.DocumentTemplateDtos
{
    public class DocumentTemplateListDto
    {
        public Guid TemplateId { get; set; }

        public string? TemplateType { get; set; }

        public string TemplateName { get; set; } = null!;

        public Guid AuthorId { get; set; }

        public string? Url { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ListUserResponseDto Author { get; set; } = null!;
    }
}
