using AutoMapper;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.BudgetProposalDtos;
using LMCM_BE.DTOs.DocumentTemplateDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.UserRepositoriy;
using LMCM_BE.Services.GoogleDriveService;
using LMCM_BE.Utilities;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.DocumentTemplateRepository
{
    public class DocumentTemplateRepository : IDocumentTemplateRepository
    {
        private readonly LMCM_DBContext _dbContext;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IMapper _mapper;
        private readonly IFileHelper _fileHelper;
        private readonly IUserRepository _userRepositoriy;

        public DocumentTemplateRepository(LMCM_DBContext dbContext, IGoogleDriveService googleDriveService, IMapper mapper, IFileHelper fileHelper, IUserRepository userRepository)
        {
            _dbContext = dbContext;
            _googleDriveService = googleDriveService;
            _mapper = mapper;
            _fileHelper = fileHelper;
            _userRepositoriy = userRepository;
        }
        public async Task<bool> CreateTemplatelAsync(DocumentTemplateInsertDto template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template), "Template data is required.");
            }
            UserProfileResponseDto user = await _userRepositoriy.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new Exception("User not found");
            // Step 1: Upload template file to Google Drive (if provided)
            string? fileUrl = null;

            if (template.File != null)
            {
                // Check file type
                if (template.File.ContentType != "application/pdf")
                {
                    throw new Exception("Only PDF files are allowed.");
                }

                // Check file size (5MB = 5 * 1024 * 1024 bytes)
                if (template.File.Length > 5 * 1024 * 1024)
                {
                    throw new Exception("File size must not exceed 5MB.");
                }

                fileUrl = await _googleDriveService.UploadDocumentTemplateFileAsync(template.File);

                if (string.IsNullOrWhiteSpace(fileUrl))
                {
                    throw new Exception("Failed to upload the file.");
                }
                else
                {
                    await _googleDriveService.SharePdfFileWithUser(fileUrl, user.Email);
                }
            }

            // Step 2: Create Template object
            var newTemplate = _mapper.Map<DocumentTemplate>(template);

            newTemplate.TemplateId = Guid.NewGuid();
            newTemplate.Url = fileUrl;
            newTemplate.AuthorId = user.Id;
            newTemplate.Status = "Active";
            newTemplate.CreatedAt = DateTime.UtcNow;
            newTemplate.UpdatedAt = DateTime.UtcNow;

            // Step 3: Save to database
            _dbContext.DocumentTemplates.Add(newTemplate);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<DocumentTemplateDetailDto> GetTemplateByIdAsync(Guid templateId)
        {
            if (templateId == Guid.Empty)
                throw new ArgumentException("Template ID cannot be empty.", nameof(templateId));

            var template = await _dbContext.DocumentTemplates
                .AsNoTracking()
                .Include(s => s.Author)
                .Where(s => s.TemplateId == templateId)
                .SingleOrDefaultAsync();

            if (template == null)
                throw new KeyNotFoundException($"No template found with ID: {templateId}");

            UserProfileResponseDto user = await _userRepositoriy.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new Exception("User not found");
            if (user.Id != template.AuthorId && user.Roles.Contains("Staff"))
                throw new UnauthorizedAccessException("User is not authorized to view this template.");

            var templateDto = _mapper.Map<DocumentTemplateDetailDto>(template);
            templateDto.DownloadUrl = await _googleDriveService.GetDownloadUrl(template.Url);

            return templateDto;
        }

        public async Task<PagedResult<DocumentTemplateListDto>> GetTemplatesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.DocumentTemplates.AsQueryable();

            UserProfileResponseDto user = await _userRepositoriy.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new Exception("User not found");
            if (user.Roles.Contains("Staff")) query = query.Where(s => s.AuthorId == user.Id);

            query = query.Where(s => s.Status != "Inactive");

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                query = query.Where(s => s.Author.Email.ToLower().Contains(search) ||
                                         s.TemplateName.ToLower().Contains(search) ||
                                         s.Author.UserName.ToLower().Contains(search)||
                                         s.TemplateType.ToLower().Contains(search));
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Include(s => s.Author)
                .OrderByDescending(s => s.UpdatedAt)
                .ToListAsync();

            var data = _mapper.Map<List<DocumentTemplateListDto>>(items);

            return new PagedResult<DocumentTemplateListDto>
            {
                Items = data,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<bool> SoftDeleteTemplateAsync(Guid templateId)
        {
            var template = await _dbContext.DocumentTemplates
               .FirstOrDefaultAsync(ar => ar.TemplateId == templateId && ar.Status == "Active");

            if (template == null)
                throw new KeyNotFoundException("Data not found.");

            UserProfileResponseDto user = await _userRepositoriy.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new Exception("User not found");
            if (user.Id != template.AuthorId && user.Roles.Contains("Staff"))
                throw new UnauthorizedAccessException("User is not authorized to update this budget proposal.");

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                template.Status = "Inactive";
                template.UpdatedAt = DateTime.UtcNow;
                _dbContext.DocumentTemplates.Update(template);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Guid?> UpdateTempalteAsync(Guid templateId, DocumentTemplateUpdateDto newTemplate)
        {
            if (templateId == Guid.Empty)
                throw new ArgumentException("Template ID cannot be empty.", nameof(templateId));

            if (newTemplate == null)
                throw new ArgumentNullException(nameof(newTemplate), "New template data cannot be null.");

            var template = await _dbContext.DocumentTemplates
                .Include(lm => lm.Author)
                .FirstOrDefaultAsync(lm => lm.TemplateId == templateId);

            if (template == null)
                throw new KeyNotFoundException($"No template found with ID: {templateId}");

            UserProfileResponseDto user = await _userRepositoriy.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new Exception("User not found");
            if (user.Id != template.AuthorId && user.Roles.Contains("Staff"))
                throw new UnauthorizedAccessException("User is not authorized to update this template.");

            // Update template fields (excluding file)
            _mapper.Map(newTemplate, template);
            template.UpdatedAt = DateTime.UtcNow;

            string? fileUrl = null;

            if (newTemplate.File != null)
            {
                // Validate file type
                if (newTemplate.File.ContentType != "application/pdf")
                    throw new InvalidOperationException("Only PDF files are allowed.");

                // Validate file size (5MB limit)
                const int maxFileSize = 5 * 1024 * 1024;
                if (newTemplate.File.Length > maxFileSize)
                    throw new InvalidOperationException("File size must not exceed 5MB.");

                // Validate if the new file is different from the existing one
                var uploadedFileHash = await _fileHelper.ComputeFileHashAsync(newTemplate.File);
                var existingFileHash = await _googleDriveService.ComputeGoogleDriveFileHashAsync(template.Url);

                if (uploadedFileHash != existingFileHash)
                {
                    fileUrl = await _googleDriveService.UploadDocumentTemplateFileAsync(newTemplate.File);
                    if (string.IsNullOrWhiteSpace(fileUrl))
                        throw new Exception("Failed to upload the file.");

                    await _googleDriveService.SharePdfFileWithUser(fileUrl, user.Email);

                    // Update the proposal's file URL **only if a new file was uploaded**
                    template.Url = fileUrl;
                }
            }

            try
            {
                await _dbContext.SaveChangesAsync();
                return template.TemplateId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating template: {ex.Message}");
                return null;
            }
        }
    }
}
