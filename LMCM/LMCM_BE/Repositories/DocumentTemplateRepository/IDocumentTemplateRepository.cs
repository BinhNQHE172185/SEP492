using LMCM_BE.Models;
namespace LMCM_BE.Repositories.DocumentTemplateRepository
{
    public interface IDocumentTemplateRepository
    {
        Task<bool> CreateTemplatelAsync(DocumentTemplate template);
        Task<(List<DocumentTemplate>,int totalCount)> GetTemplatesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10);
        Task<DocumentTemplate> GetTemplateByIdAsync(Guid templateId);
        Task<bool> UpdateTemplateAsync(DocumentTemplate newTemplate);
    }
}
