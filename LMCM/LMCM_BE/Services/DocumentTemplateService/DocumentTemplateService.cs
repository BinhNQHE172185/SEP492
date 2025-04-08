using AutoMapper;
using LMCM_BE.DTOs.DocumentTemplateDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Models;
using LMCM_BE.Models.Constant;
using LMCM_BE.Repositories.DocumentTemplateRepository;
using LMCM_BE.Services.GoogleDriveService;
using LMCM_BE.Services.UserService;
using LMCM_BE.UnitOfWork;
using LMCM_BE.Utilities;

namespace LMCM_BE.Services.DocumentTemplateService
{
    public class DocumentTemplateService : IDocumentTemplateService
    {
        private readonly IDocumentTemplateRepository _documentTemplateRepository;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IMapper _mapper;
        private readonly IFileHelper _fileHelper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;   
        public DocumentTemplateService(IDocumentTemplateRepository documentTemplateRepository,
            IGoogleDriveService googleDriveService, IMapper mapper, IFileHelper fileHelper,IUnitOfWork unitOfWork,
            IUserService userService)
        {
            _documentTemplateRepository = documentTemplateRepository;
            _googleDriveService = googleDriveService;
            _mapper = mapper;
            _fileHelper = fileHelper;
            _unitOfWork = unitOfWork;   
            _userService = userService;
        }
        public async Task<bool> CreateTemplatelAsync(DocumentTemplateInsertDto template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template), "Template data is required.");
            }
            UserProfileResponseDto user =await _userService.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new Exception("User not found");
            // Step 1: Upload template file to Google Drive (if provided)
            string? fileUrl = null;

            if (template.File != null)
            {

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
            newTemplate.CreatedAt = DateTime.UtcNow;
            newTemplate.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _documentTemplateRepository.CreateTemplatelAsync(newTemplate);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        public async Task<DocumentTemplateDetailDto> GetTemplateByIdAsync(Guid templateId)
        {
            if (templateId == Guid.Empty)
                throw new ArgumentException("ID bị trống.", nameof(templateId));
            
            var template= await _documentTemplateRepository.GetTemplateByIdAsync(templateId);

            if (template == null)
                throw new KeyNotFoundException($"Không tìm được mẫu với ID: {templateId}");

            var templateDto = _mapper.Map<DocumentTemplateDetailDto>(template);
            templateDto.DownloadUrl = await _googleDriveService.GetDownloadUrl(template.Url);

            return templateDto;
        }

        public async Task<PagedResult<DocumentTemplateListDto>> GetTemplatesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var (items, totalCount) = await _documentTemplateRepository.GetTemplatesAsync(searchKey, pageIndex, pageSize);
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
            var template = await _documentTemplateRepository.GetTemplateByIdAsync(templateId);

            if (template == null)
                throw new KeyNotFoundException("Không tìm thấy dữ liệu.");

            UserProfileResponseDto user = await _userService.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new UnauthorizedAccessException("Không tìm thấy người dùng");
            if (user.Id != template.AuthorId && user.Roles.Contains("Staff"))
                throw new UnauthorizedAccessException("Người dùng không có quyền xóa mẫu tài liệu.");

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                template.Status = DocumentTemplateStatus.Deleted;
                template.UpdatedAt = DateTime.UtcNow;  
                await _documentTemplateRepository.UpdateTemplateAsync(template);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateTempalteAsync(Guid templateId, DocumentTemplateUpdateDto newTemplate)
        {
            if (templateId == Guid.Empty)
                throw new ArgumentNullException("Id không được trống.", nameof(templateId));

            if (newTemplate == null)
                throw new ArgumentNullException(nameof(newTemplate), "Dữ liệu mới không được null.");

            var template = await _documentTemplateRepository.GetTemplateByIdAsync(templateId);

            if (template == null)
                throw new KeyNotFoundException($"Không tìm thấy mẫu tài liệu với ID: {templateId}");

            UserProfileResponseDto user = await _userService.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new Exception("Không tìm thấy người dùng");
            if (user.Id != template.AuthorId && user.Roles.Contains("Staff"))
                throw new UnauthorizedAccessException("Người dùng không có quyền cập nhật mẫu tài liệu.");

            _mapper.Map(newTemplate, template);
            template.UpdatedAt = DateTime.UtcNow;

            string? fileUrl = null;

            if (newTemplate.File != null)
            {
                // Validate if the new file is different from the existing one
                var uploadedFileHash = await _fileHelper.ComputeFileHashAsync(newTemplate.File);
                var existingFileHash = await _googleDriveService.ComputeGoogleDriveFileHashAsync(template.Url);

                if (uploadedFileHash != existingFileHash)
                {
                    fileUrl = await _googleDriveService.UploadBudgetProposalFileAsync(newTemplate.File);
                    if (string.IsNullOrWhiteSpace(fileUrl))
                        throw new Exception("Tải file thất bại.");

                    await _googleDriveService.SharePdfFileWithUser(fileUrl, user.Email);

                    // Update the proposal's file URL **only if a new file was uploaded**
                    template.Url = fileUrl;
                }
            }
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _documentTemplateRepository.UpdateTemplateAsync(template);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
    }
}
