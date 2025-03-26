namespace LMCM_BE.DTOs.DocumentTemplateDtos
{
    public class DocumentTemplateUpdateDto
    {
        public string? TemplateType { get; set; }

        public string TemplateName { get; set; } = null!;
        public IFormFile? File { get; set; }
    }
}
