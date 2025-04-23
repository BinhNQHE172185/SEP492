using AutoMapper;
using LMCM_BE.DTOs.AcceptanceRecordDtos;
using LMCM_BE.DTOs.BudgetProposalDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.AcceptanceRecordRepository;
using LMCM_BE.Repositories.ContractRepository;
using LMCM_BE.Repositories.UserRepositoriy;
using LMCM_BE.Services.GoogleDriveService;
using LMCM_BE.Services.UserService;
using LMCM_BE.Shared.Constant;
using LMCM_BE.UnitOfWork;
using LMCM_BE.Utilities;
using System.Collections.Concurrent;

namespace LMCM_BE.Services.AcceptanceRecordService
{
    public class AcceptanceRecordService : IAcceptanceRecordService
    {
        private readonly IAcceptanceRecordRepository _acceptanceRecordRepository;
        private readonly IMapper _mapper;
        private readonly IContractRepository _contractRepository;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IFileHelper _fileHelper;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _acceptanceRecordFolderId;
        private static readonly ConcurrentDictionary<string, bool> _fileLocks = new();
        public AcceptanceRecordService(
            IAcceptanceRecordRepository 
            acceptanceRecordRepository, 
            IMapper mapper,
            IContractRepository contractRepository,
            IGoogleDriveService googleDriveService,
            IFileHelper fileHelper,
            IUserService userService,
            IUnitOfWork unitOfWork,
            IConfiguration configuration
            )
        {
            _acceptanceRecordRepository = acceptanceRecordRepository;
            _mapper = mapper;
            _contractRepository = contractRepository;
            _googleDriveService = googleDriveService;
            _fileHelper = fileHelper;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _acceptanceRecordFolderId = configuration["GoogleDriveFolders:AcceptanceRecord"];
        }
        public async Task<PagedResult<AcceptanceRecordListDto>> GetAcceptanceRecordsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            UserProfileResponseDto user = await _userService.GetProfileFromCookie();
            bool isHod = false;
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new UnauthorizedAccessException("Không tìm thấy người dùng");
            if (!user.Roles.Contains("Head of Department")) isHod = true;

            var (items, totalCount) = await _acceptanceRecordRepository.GetAcceptanceRecordsAsync(isHod, user.Id, searchKey, pageIndex, pageSize);
            var data = _mapper.Map<List<AcceptanceRecordListDto>>(items);

            return new PagedResult<AcceptanceRecordListDto>
            {
                Items = data,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }
        public async Task<bool> CreateAcceptanceRecordAsync(AcceptanceRecordCreateDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "Dữ liệu tờ trình là bắt buộc.");
            }

            if (await _acceptanceRecordRepository.GetDuplicatedTitleIdAsync(dto.Title) != null)
            {
                throw new InvalidOperationException("Tiêu đề đã tồn tại.");
            }

            UserProfileResponseDto user = await _userService.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new UnauthorizedAccessException("Không tìm thấy người dùng");

            if (await _contractRepository.GetActiveContractByIdAsync(dto.ContractId) == null)
            {
                throw new KeyNotFoundException("Hợp đồng được chọn không tồn tại");
            }

            // Generate a unique lock key for the user and contract title combination
            var lockKey = $"{user.Id}_{dto.Title}";

            // Check if the contract is being processed already
            if (!_fileLocks.TryAdd(lockKey, true))  // Try to add a lock
            {
                throw new InvalidOperationException("Biên bản nghiệm thu này đang được xử lý. Vui lòng đợi.");
            }

            string? fileUrl = null;

            if (dto.File != null)
            {
                fileUrl = await _googleDriveService.UploadFileAsync(dto.File,_acceptanceRecordFolderId);

                if (string.IsNullOrWhiteSpace(fileUrl))
                {
                    throw new Exception("Tải file thất bại.");
                }
                else
                {
                    await _googleDriveService.SharePdfFileWithUserAsync(fileUrl, user.Email, "reader");
                }
            }

            // Step 2: Create record
            var acceptanceRecord = _mapper.Map<AcceptanceRecord>(dto);
            acceptanceRecord.AuthorId = user.Id;
            acceptanceRecord.Url = fileUrl;
            acceptanceRecord.AcceptanceId = Guid.NewGuid();
            acceptanceRecord.Status = GenericStatus.Active;
            acceptanceRecord.CreatedAt = DateTime.UtcNow;
            acceptanceRecord.UpdatedAt = DateTime.UtcNow;
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _acceptanceRecordRepository.CreateAcceptanceRecordAsync(acceptanceRecord);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
        public async Task<Guid?> UpdateAcceptanceRecordAsync(Guid acceptanceId, AcceptanceRecordUpdateDto dto)
        {
            if (acceptanceId == Guid.Empty)
                throw new ArgumentException("ID biên bản nghiệm thu không được để trống.", nameof(acceptanceId));

            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Dữ liệu cập nhật không được để trống.");

            var duplicateId = await _acceptanceRecordRepository.GetDuplicatedTitleIdAsync(dto.Title);
            if (duplicateId != null && duplicateId != acceptanceId)
            {
                throw new InvalidOperationException("Tiêu đề đã tồn tại.");
            }

            var acceptanceRecord = await _acceptanceRecordRepository.GetActiveAcceptanceRecordByIdAsync(acceptanceId);

            if (acceptanceRecord == null)
                throw new KeyNotFoundException("Không tìm thấy biên bản nghiệm thu.");

            if (await _contractRepository.GetActiveContractByIdAsync(dto.ContractId) == null)
            {
                throw new KeyNotFoundException("Hợp đồng được chọn không tồn tại");
            }

            UserProfileResponseDto user = await _userService.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new UnauthorizedAccessException("Không tìm thấy người dùng");
            if (user.Id != acceptanceRecord.AuthorId && user.Roles.Contains("Staff"))
                throw new UnauthorizedAccessException("Người dùng không có quyền cập nhật biên bản nghiệm thu này.");

            _mapper.Map(dto, acceptanceRecord);
            acceptanceRecord.UpdatedAt = DateTime.UtcNow;

            string? fileUrl = null;

            if (dto.File != null)
            {
                // Validate if the new file is different from the existing one
                var uploadedFileHash = await _fileHelper.ComputeFileHashAsync(dto.File);
                var existingFileHash = await _googleDriveService.ComputeGoogleDriveFileHashAsync(acceptanceRecord.Url);

                if (uploadedFileHash != existingFileHash)
                {
                    fileUrl = await _googleDriveService.UploadFileAsync(dto.File, _acceptanceRecordFolderId);
                    if (string.IsNullOrWhiteSpace(fileUrl))
                        throw new Exception("Tải file thất bại.");

                    await _googleDriveService.SharePdfFileWithUserAsync(fileUrl, user.Email,"reader");

                    // Update the proposal's file URL **only if a new file was uploaded**
                    acceptanceRecord.Url = fileUrl;
                }
            }
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _acceptanceRecordRepository.UpdateAcceptanceRecordAsync(acceptanceRecord);
                await _unitOfWork.CommitAsync();
                return acceptanceId;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> SoftDeleteAcceptanceRecordAsync(Guid acceptanceId)
        {
            if (acceptanceId == Guid.Empty)
                throw new ArgumentException("ID biên bản nghiệm thu không được để trống.", nameof(acceptanceId));

            var acceptanceRecord = await _acceptanceRecordRepository.GetActiveAcceptanceRecordByIdAsync(acceptanceId);

            if (acceptanceRecord == null)
                throw new KeyNotFoundException("Không tìm thấy biên bản nghiệm thu hoặc đã bị xóa.");

            UserProfileResponseDto user = await _userService.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new UnauthorizedAccessException("Không tìm thấy người dùng");
            if (user.Id != acceptanceRecord.AuthorId && user.Roles.Contains("Staff"))
                throw new UnauthorizedAccessException("Người dùng không có quyền xóa biên bản nghiệm thu này.");

            acceptanceRecord.Status = GenericStatus.Inactive;
            acceptanceRecord.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _acceptanceRecordRepository.UpdateAcceptanceRecordAsync(acceptanceRecord);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
        public async Task<AcceptanceRecordDetailDto> GetAcceptanceRecordDetailAsync(Guid acceptanceId)
        {
            if (acceptanceId == Guid.Empty)
                throw new ArgumentException("Biên bản nghiệm thu ID không được để trống.");

            var acceptanceRecord = await _acceptanceRecordRepository.GetAcceptanceRecordDetailAsync(acceptanceId);

            if (acceptanceRecord == null)
                throw new KeyNotFoundException("Không tìm thấy biên bản nghiệm thu.");

            UserProfileResponseDto user = await _userService.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new UnauthorizedAccessException("Không tìm thấy người dùng");
            if (user.Id != acceptanceRecord.AuthorId && user.Roles.Contains("Staff"))
                throw new UnauthorizedAccessException("Người dùng không có quyền xem biên bản nghiệm thu này.");

            var acceptanceRecordDto = _mapper.Map<AcceptanceRecordDetailDto>(acceptanceRecord);
            acceptanceRecordDto.DownloadUrl = await _googleDriveService.GetDownloadUrlAsync(acceptanceRecord.Url);
            return acceptanceRecordDto;
        }
    }
}
