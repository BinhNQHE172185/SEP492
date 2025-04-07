using AutoMapper;
using LMCM_BE.DTOs.ContractDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.AcceptanceRecordRepository;
using LMCM_BE.Repositories.ContractRepository;
using LMCM_BE.Services.GoogleDriveService;
using LMCM_BE.Services.UserService;
using LMCM_BE.UnitOfWork;
using LMCM_BE.Utilities;

namespace LMCM_BE.Services.ContractService
{
    public class ContractService : IContractService
    {
        private readonly IContractRepository _contractRepository;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IMapper _mapper;
        private readonly IFileHelper _fileHelper;
        private readonly IAcceptanceRecordRepository _acceptanceRecordRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public ContractService(IContractRepository contractRepository, IGoogleDriveService googleDriveService,
            IMapper mapper, IFileHelper fileHelper, IAcceptanceRecordRepository acceptanceRecordRepository,
            IUnitOfWork unitOfWork,IUserService userService)
        {
            _contractRepository = contractRepository;
            _googleDriveService = googleDriveService;
            _mapper = mapper;
            _fileHelper = fileHelper;
            _acceptanceRecordRepository = acceptanceRecordRepository;
            _unitOfWork = unitOfWork;
            _userService = userService;
        }
        public async Task<bool> CreateContract( ContractInsertDto contractDto)
        {
            UserProfileResponseDto user =await _userService.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new Exception("User not found");
            // Step 1: Upload contract file to Google Drive (if provided)
            string? fileUrl = null;
            if (contractDto.File != null)
            {
                fileUrl = await _googleDriveService.UploadContractFileAsync(contractDto.File);

                if (string.IsNullOrWhiteSpace(fileUrl))
                {
                    throw new Exception("Failed to upload the file.");
                }
                else
                {
                    await _googleDriveService.SharePdfFileWithUser(fileUrl, user.Email);
                }
            }

            // Step 2: Create Contract object
            var newContract = _mapper.Map<Contract>(contractDto);
            newContract.Url = fileUrl;
            newContract.AuthorId = user.Id;

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _contractRepository.CreateContract(newContract);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
        public async Task<ContractDetailDto> GetContractByIdAsync( Guid contractId)
        {
            if (contractId == Guid.Empty)
                throw new ArgumentNullException("ID bị trống.", nameof(contractId));
            var contract = await _contractRepository.GetContractDetailByIdAsync(contractId);
            if (contract == null)
                throw new KeyNotFoundException($"Không tìm thấy hợp đồn với ID: {contractId}");

            UserProfileResponseDto user = await _userService.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new Exception("Không tìm thấy người dùng");
            if (user.Id != contract.AuthorId && user.Roles.Contains("Staff"))
                throw new UnauthorizedAccessException("Người dùng không có quyền xem hợp đồng này.");

            var contractDto = _mapper.Map<ContractDetailDto>(contract);
            contractDto.DownloadUrl = await _googleDriveService.GetDownloadUrl(contract.Url);

            return contractDto;
        }

        public async Task<PagedResult<ContractListDto>> GetContractsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            bool isHod = false;
            UserProfileResponseDto user = await _userService.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new Exception("Không tìm thấy người dùng");
            if (user.Roles.Contains("Head of Department")) isHod = true;

            var (items, totalCount) = await _contractRepository.GetContractsAsync(isHod, user.Id, searchKey, pageIndex, pageSize);
            var data = _mapper.Map<List<ContractListDto>>(items);

            return new PagedResult<ContractListDto>
            {
                Items = data,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<List<ContractListDto>> GetContractsAsync(string? searchKey)
        {
            bool isHod = false;
            UserProfileResponseDto user = await _userService.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new Exception("Không tìm thấy người dùng");
            if (user.Roles.Contains("Head of Department")) isHod = true;

            var items = await _contractRepository.GetContractsAsync(isHod, user.Id, searchKey);

            var data = _mapper.Map<List<ContractListDto>>(items);

            return data;
        }

        public async Task<bool> SoftDeleteContractAsync(Guid contractId)
        {
            var contract = await _contractRepository.GetActiveContractByIdAsync(contractId);

            if (contract == null)
                throw new KeyNotFoundException("Không tìm thấy dữ liệu.");

            UserProfileResponseDto user = await _userService.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new UnauthorizedAccessException("Không tìm thấy người dùng");
            if (user.Id != contract.AuthorId && user.Roles.Contains("Staff"))
                throw new UnauthorizedAccessException("Người dùng không có quyền xóa tờ trình.");

            if (await _acceptanceRecordRepository.HasActiveAcceptanceRecordsAsync(contractId))
            {
                throw new InvalidOperationException("Không thể xóa do có tờ trình lệ thuộc");
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _contractRepository.SoftDeleteContractAsync(contract);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateContractAsync( Guid contractId, ContractUpdateDto newContract)
        {
            if (contractId == Guid.Empty)
                throw new ArgumentNullException("Id không được trống.", nameof(contractId));

            if (newContract == null)
                throw new ArgumentNullException(nameof(newContract), "Dữ liệu mới không được null.");

            var contract = await _contractRepository.GetContractByIdAsync(contractId);

            if (contract == null)
                throw new KeyNotFoundException($"Không tìm thấy hợp đồng với ID: {contractId}");

            UserProfileResponseDto user = await _userService.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new Exception("Không tìm thấy người dùng");
            if (user.Id != contract.AuthorId && user.Roles.Contains("Staff"))
                throw new UnauthorizedAccessException("Người dùng không có quyền cập nhật hợp đồng.");

            _mapper.Map(newContract, contract);
            contract.UpdatedAt = DateTime.UtcNow;

            string? fileUrl = null;

            if (newContract.File != null)
            {
                // Validate if the new file is different from the existing one
                var uploadedFileHash = await _fileHelper.ComputeFileHashAsync(newContract.File);
                var existingFileHash = await _googleDriveService.ComputeGoogleDriveFileHashAsync(contract.Url);

                if (uploadedFileHash != existingFileHash)
                {
                    fileUrl = await _googleDriveService.UploadBudgetProposalFileAsync(newContract.File);
                    if (string.IsNullOrWhiteSpace(fileUrl))
                        throw new Exception("Tải file thất bại.");

                    await _googleDriveService.SharePdfFileWithUser(fileUrl, user.Email);

                    // Update the proposal's file URL **only if a new file was uploaded**
                    contract.Url = fileUrl;
                }
            }
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _contractRepository.UpdateContractAsync(contract);
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
