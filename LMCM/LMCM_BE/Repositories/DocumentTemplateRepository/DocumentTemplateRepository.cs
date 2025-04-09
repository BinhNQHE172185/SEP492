using LMCM_BE.DbContext;
using LMCM_BE.Models;
using LMCM_BE.Shared.Constant;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.DocumentTemplateRepository
{
    public class DocumentTemplateRepository : IDocumentTemplateRepository
    {
        private readonly LMCM_DBContext _dbContext;

        public DocumentTemplateRepository(LMCM_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> CreateTemplatelAsync(DocumentTemplate template)
        {
            await _dbContext.DocumentTemplates.AddAsync(template);

            return true;
        }

        public async Task<DocumentTemplate> GetTemplateByIdAsync(Guid templateId)
        {
            var template = await _dbContext.DocumentTemplates
                .Include(s => s.Author)
                .Where(s => s.TemplateId == templateId)
                .SingleOrDefaultAsync();

            return template;
        }

        public async Task<(List<DocumentTemplate>, int totalCount)> GetTemplatesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.DocumentTemplates.AsQueryable();

            query = query.OrderByDescending(s => s.UpdatedAt);

            query = query.Where(s => s.Status == DocumentTemplateStatus.Active);

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                query = query.Where(s => s.Author.Email.ToLower().Contains(search) ||
                                         s.TemplateName.ToLower().Contains(search) ||
                                         s.Author.UserName.ToLower().Contains(search) ||
                                         s.TemplateType.ToLower().Contains(search)||
                                         s.Status.ToString().ToLower().Contains(search));
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Include(s => s.Author)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<bool> UpdateTemplateAsync(DocumentTemplate newTemplate)
        {
            _dbContext.DocumentTemplates.Update(newTemplate);
            return true;
        }
    }
}
