using LMCM_BE.DTOs.DocumentTemplateDtos;
using LMCM_BE.DTOs.ShareDtos;

namespace LMCM_BE.Repositories.DocumentTemplateRepository
{
    public interface IDocumentTemplateRepository
    {
        Task<bool> CreateTemplatelAsync(DocumentTemplateInsertDto template);
        Task<PagedResult<DocumentTemplateListDto>> GetTemplatesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<DocumentTemplateDetailDto> GetTemplateByIdAsync(Guid templateId);
        Task<bool> SoftDeleteTemplateAsync(Guid templateId);
        Task<Guid?> UpdateTempalteAsync(Guid templateId, DocumentTemplateUpdateDto newTemplate);
    }
}
