using LMCM_BE.DTOs.UserDtos;

namespace LMCM_BE.DTOs.DocumentTemplateDtos
{
    public class DocumentTemplateInsertDto
    {
        public string? TemplateType { get; set; }

        public string TemplateName { get; set; } = null!;
        public IFormFile? File { get; set; }
    }
}
