using AutoMapper;
using LMCM_BE.DTOs.BudgetProposalDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.BudgetPropasalRepository;
using LMCM_BE.Repositories.ContractRepository;
using LMCM_BE.Services.GoogleDriveService;
using LMCM_BE.Services.UserService;
using LMCM_BE.Shared.Constant;
using LMCM_BE.UnitOfWork;

namespace LMCM_BE.Services.BudgetPropasalService
{
    public class BudgetProposalService : IBudgetProposalService
    {
        private readonly IBudgetProposalRepository _budgetProposalRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly string _budgetProposalFolderId;

        public BudgetProposalService(IBudgetProposalRepository budgetPropasalRepository, IContractRepository contractRepository,
            IGoogleDriveService googleDriveService, IMapper mapper,
            IUnitOfWork unitOfWork, IUserService userService, IConfiguration configuration)
        {
            _budgetProposalRepository = budgetPropasalRepository;
            _contractRepository = contractRepository;
            _googleDriveService = googleDriveService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userService = userService;
            _budgetProposalFolderId = configuration["GoogleDriveFolders:BudgetProposal"];
        }
        public async Task<bool> CreateBudgetProposalAsync(BudgetProposalInsertDto proposal)
        {
            if (proposal == null)
            {
                throw new ArgumentNullException(nameof(proposal), "Dữ liệu tờ trình là bắt buộc.");
            }

            if (await _budgetProposalRepository.GetDuplicatedTitleIdAsync(proposal.Title) != null)
            {
                throw new InvalidOperationException("Tiêu đề đã tồn tại.");
            }

            UserProfileResponseDto user = await _userService.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new UnauthorizedAccessException("Không tìm thấy người dùng");
            // Step 1: Upload contract file to Google Drive (if provided)
            string? fileUrl = null;

            if (proposal.File != null)
            {
                fileUrl = await _googleDriveService.UploadFileAsync(proposal.File, _budgetProposalFolderId);

                if (string.IsNullOrWhiteSpace(fileUrl))
                {
                    throw new Exception("Tải file thất bại.");
                }
                else
                {
                    await _googleDriveService.SharePdfFileWithUserAsync(fileUrl, user.Email, "reader");
                }
            }
            // Step 2: Create Contract object
            var newProposal = _mapper.Map<BudgetProposal>(proposal);

            newProposal.ProposalId = Guid.NewGuid();
            newProposal.Url = fileUrl;
            newProposal.AuthorId = user.Id;
            newProposal.Status = GenericStatus.Active;
            newProposal.CreatedAt = DateTime.UtcNow;
            newProposal.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _budgetProposalRepository.CreateBudgetProposalAsync(newProposal);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        public async Task<BudgetProposalDetailDto> GetBudgetProposalByIdAsync(Guid proposalId)
        {
            if (proposalId == Guid.Empty)
                throw new ArgumentNullException("Proposal ID bị trống.", nameof(proposalId));

            var budgetProposal = await _budgetProposalRepository.GetBudgetProposalByIdAsync(proposalId);

            if (budgetProposal == null)
                throw new KeyNotFoundException($"Không tìm thấy tờ trình với ID: {proposalId}");

            UserProfileResponseDto user = await _userService.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new UnauthorizedAccessException("Không tìm thấy người dùng");
            if (user.Id != budgetProposal.AuthorId && user.Roles.Contains("Staff"))
                throw new UnauthorizedAccessException("Người dùng không có quyền xem tờ trình.");

            var budgetProposalDto = _mapper.Map<BudgetProposalDetailDto>(budgetProposal);
            budgetProposalDto.DownloadUrl = await _googleDriveService.GetDownloadUrlAsync(budgetProposal.Url);
            return budgetProposalDto;
        }

        public async Task<PagedResult<BudgetProposalListDto>> GetBudgetProposalsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            bool isHod = false;
            UserProfileResponseDto user = await _userService.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new Exception("Không tìm thấy người dùng");
            if (user.Roles.Contains("Head of Department")) isHod = true;

            var (items, totalCount) = await _budgetProposalRepository.GetBudgetProposalsAsync(isHod, user.Id, searchKey, pageIndex, pageSize);
            var data = _mapper.Map<List<BudgetProposalListDto>>(items);

            return new PagedResult<BudgetProposalListDto>
            {
                Items = data,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<List<BudgetProposalListDto>> GetBudgetProposalsAsync(string? searchKey)
        {
            bool isHod = false;
            UserProfileResponseDto user = await _userService.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new UnauthorizedAccessException("Không tìm thấy người dùng");
            if (user.Roles.Contains("Head of Department")) isHod = true;

            var items = await _budgetProposalRepository.GetBudgetProposalsAsync(isHod, user.Id, searchKey);

            var data = _mapper.Map<List<BudgetProposalListDto>>(items);

            return data;
        }

        public async Task<bool> SoftDeleteBudgetProposalAsync(Guid proposalId)
        {
            var budgetProposal = await _budgetProposalRepository.GetActiveBudgetProposalByIdAsync(proposalId);

            if (budgetProposal == null)
                throw new KeyNotFoundException("Không tìm thấy dữ liệu.");

            UserProfileResponseDto user = await _userService.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new UnauthorizedAccessException("Không tìm thấy người dùng");
            if (user.Id != budgetProposal.AuthorId && user.Roles.Contains("Staff"))
                throw new UnauthorizedAccessException("Người dùng không có quyền xóa tờ trình.");

            if (await _contractRepository.HasActiveConntractsAsync(proposalId))
            {
                throw new InvalidOperationException("Không thể xóa do có hợp đồng lệ thuộc");
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                budgetProposal.Status = GenericStatus.Inactive;
                budgetProposal.UpdatedAt = DateTime.UtcNow;

                await _budgetProposalRepository.UpdateBudgetProposalAsync(budgetProposal);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateBudgetProposalAsync(Guid proposalId, BudgetProposalUpdateDto newProposal)
        {
            if (proposalId == Guid.Empty)
                throw new ArgumentNullException("Id không được trống.", nameof(proposalId));

            if (newProposal == null)
                throw new ArgumentNullException(nameof(newProposal), "Dữ liệu mới không được null.");

            var duplicateId = await _budgetProposalRepository.GetDuplicatedTitleIdAsync(newProposal.Title);
            if (duplicateId != null && duplicateId != proposalId)
            {
                throw new InvalidOperationException("Tiêu đề đã tồn tại.");
            }

            var proposal = await _budgetProposalRepository.GetActiveBudgetProposalByIdAsync(proposalId);

            if (proposal == null)
                throw new KeyNotFoundException($"Không tìm thấy tờ trình với ID: {proposalId}");

            UserProfileResponseDto user = await _userService.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new Exception("Không tìm thấy người dùng");
            if (user.Id != proposal.AuthorId && user.Roles.Contains("Staff"))
                throw new UnauthorizedAccessException("Người dùng không có quyền cập nhật tờ trình.");

            _mapper.Map(newProposal, proposal);
            proposal.UpdatedAt = DateTime.UtcNow;

            string? fileUrl = null;

            if (newProposal.File != null)
            {
                fileUrl = await _googleDriveService.UploadFileAsync(newProposal.File, _budgetProposalFolderId);
                if (string.IsNullOrWhiteSpace(fileUrl))
                    throw new Exception("Tải file thất bại.");

                await _googleDriveService.SharePdfFileWithUserAsync(fileUrl, user.Email, "reader");

                // Update the proposal's file URL **only if a new file was uploaded**
                proposal.Url = fileUrl;

            }
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _budgetProposalRepository.UpdateBudgetProposalAsync(proposal);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
        public async Task<int> BudgetCountAsync()
        {
            var count = await _budgetProposalRepository.BudgetCountAsync();
            return count;
        }
    }
}
