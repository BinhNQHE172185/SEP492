using LMCM_BE.DTOs.DocumentTemplateDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Repositories.DocumentTemplateRepository;

namespace LMCM_BE.Services.DocumentTemplateService
{
    public class DocumentTemplateService : IDocumentTemplateService
    {
        private readonly IDocumentTemplateRepository _documentTemplateRepository;

        public DocumentTemplateService(IDocumentTemplateRepository documentTemplateRepository)
        {
            _documentTemplateRepository = documentTemplateRepository;
        }
        public async Task<bool> CreateTemplatelAsync(DocumentTemplateInsertDto template)
        {
            return await _documentTemplateRepository.CreateTemplatelAsync(template);
        }

        public async Task<DocumentTemplateDetailDto> GetTemplateByIdAsync(Guid templateId)
        {
            return await _documentTemplateRepository.GetTemplateByIdAsync(templateId);
        }

        public async Task<PagedResult<DocumentTemplateListDto>> GetTemplatesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            return await _documentTemplateRepository.GetTemplatesAsync(searchKey, pageIndex, pageSize);
        }

        public async Task<bool> SoftDeleteTemplateAsync(Guid templateId)
        {
            return await _documentTemplateRepository.SoftDeleteTemplateAsync(templateId);
        }

        public async Task<Guid?> UpdateTempalteAsync(Guid templateId, DocumentTemplateUpdateDto newTemplate)
        {
            return await _documentTemplateRepository.UpdateTempalteAsync(templateId, newTemplate);  
        }
    }
}
