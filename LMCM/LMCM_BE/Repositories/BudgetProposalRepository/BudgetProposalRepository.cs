using AutoMapper;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.BudgetProposalDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.UserRepositoriy;
using LMCM_BE.Services.GoogleDriveService;
using LMCM_BE.Utilities;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.BudgetPropasalRepository
{
    public class BudgetProposalRepository : IBudgetProposalRepository
    {
        private readonly LMCM_DBContext _dbContext;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IMapper _mapper;
        private readonly IFileHelper _fileHelper;
        private readonly IUserRepository _userRepositoriy;

        public BudgetProposalRepository(LMCM_DBContext dbContext, IGoogleDriveService googleDriveService, IMapper mapper, IFileHelper fileHelper, IUserRepository userRepository)
        {
            _dbContext = dbContext;
            _googleDriveService = googleDriveService;
            _mapper = mapper;
            _fileHelper = fileHelper;
            _userRepositoriy = userRepository;
        }
        public async Task<bool> CreateBudgetProposalAsync(BudgetProposalInsertDto proposal)
        {
            if (proposal == null)
            {
                throw new ArgumentNullException(nameof(proposal), "Proposal data is required.");
            }
            UserProfileResponseDto user = await _userRepositoriy.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new Exception("User not found");
            // Step 1: Upload contract file to Google Drive (if provided)
            string? fileUrl = null;

            if (proposal.File != null)
            {
                // Check file type
                if (proposal.File.ContentType != "application/pdf")
                {
                    throw new Exception("Only PDF files are allowed.");
                }

                // Check file size (5MB = 5 * 1024 * 1024 bytes)
                if (proposal.File.Length > 5 * 1024 * 1024)
                {
                    throw new Exception("File size must not exceed 5MB.");
                }

                fileUrl = await _googleDriveService.UploadBudgetProposalFileAsync(proposal.File);

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
            var newProposal = _mapper.Map<BudgetProposal>(proposal);

            newProposal.ProposalId = Guid.NewGuid();
            newProposal.Url = fileUrl;
            newProposal.AuthorId=user.Id;   
            newProposal.Status = "Active";
            newProposal.CreatedAt = DateTime.UtcNow;
            newProposal.UpdatedAt = DateTime.UtcNow;

            // Step 3: Save to database
            _dbContext.BudgetProposals.Add(newProposal);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<BudgetProposalDetailDto> GetBudgetProposalByIdAsync(Guid proposalId)
        {
            if (proposalId == Guid.Empty)
                throw new ArgumentException("Proposal ID cannot be empty.", nameof(proposalId));

            var budgetProposal = await _dbContext.BudgetProposals
                .AsNoTracking()
                .Include(s => s.Author)
                .Where(s => s.ProposalId == proposalId)
                .SingleOrDefaultAsync();

            if (budgetProposal == null)
                throw new KeyNotFoundException($"No budget proposal found with ID: {proposalId}");

            UserProfileResponseDto user = await _userRepositoriy.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new Exception("User not found");
            if (user.Id != budgetProposal.AuthorId && !user.Roles.Contains("Staff"))
                throw new UnauthorizedAccessException("User is not authorized to view this budget proposal.");

            var budgetProposalDto = _mapper.Map<BudgetProposalDetailDto>(budgetProposal);
            budgetProposalDto.DownloadUrl = await _googleDriveService.GetDownloadUrl(budgetProposal.Url);

            return budgetProposalDto;
        }

        public async Task<PagedResult<BudgetProposalListDto>> GetBudgetProposalsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.BudgetProposals.AsQueryable();

            UserProfileResponseDto user = await _userRepositoriy.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new Exception("User not found");
            if (!user.Roles.Contains("Admin")) query = query.Where(s => s.AuthorId == user.Id);

            query = query.Where(s => s.Status != "Inactive");

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                query = query.Where(s => s.Author.Email.ToLower().Contains(search) ||
                                         s.Title.ToLower().Contains(search) ||
                                         s.Author.UserName.ToLower().Contains(search));
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Include(s => s.Author)
                .ToListAsync();

            var data = _mapper.Map<List<BudgetProposalListDto>>(items);

            return new PagedResult<BudgetProposalListDto>
            {
                Items = data,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<bool> SoftDeleteBudgetProposalAsync(Guid proposalId)
        {
            var budgetProposal = await _dbContext.BudgetProposals
                .FirstOrDefaultAsync(ar => ar.ProposalId == proposalId && ar.Status == "Active");

            if (budgetProposal == null)
                throw new KeyNotFoundException("Data not found.");

            UserProfileResponseDto user = await _userRepositoriy.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new Exception("User not found");
            if (user.Id != budgetProposal.AuthorId)
                throw new UnauthorizedAccessException("User is not authorized to update this budget proposal.");

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                budgetProposal.Status = "Inactive";
                budgetProposal.UpdatedAt = DateTime.UtcNow;
                _dbContext.BudgetProposals.Update(budgetProposal);

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

        public async Task<Guid?> UpdateBudgetProposalAsync(Guid proposalId, BudgetProposalUpdateDto newProposal)
        {
            if (proposalId == Guid.Empty)
                throw new ArgumentException("Proposal ID cannot be empty.", nameof(proposalId));

            if (newProposal == null)
                throw new ArgumentNullException(nameof(newProposal), "New proposal data cannot be null.");

            var proposal = await _dbContext.BudgetProposals
                .Include(lm => lm.Author)
                .FirstOrDefaultAsync(lm => lm.ProposalId == proposalId);

            if (proposal == null)
                throw new KeyNotFoundException($"No budget proposal found with ID: {proposalId}");

            UserProfileResponseDto user = await _userRepositoriy.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new Exception("User not found");
            if (user.Id != proposal.AuthorId)
                throw new UnauthorizedAccessException("User is not authorized to update this proposal.");

            // Update proposal fields (excluding file)
            _mapper.Map(newProposal, proposal);
            proposal.UpdatedAt = DateTime.UtcNow;

            string? fileUrl = null;

            if (newProposal.File != null)
            {
                // Validate file type
                if (newProposal.File.ContentType != "application/pdf")
                    throw new InvalidOperationException("Only PDF files are allowed.");

                // Validate file size (5MB limit)
                const int maxFileSize = 5 * 1024 * 1024;
                if (newProposal.File.Length > maxFileSize)
                    throw new InvalidOperationException("File size must not exceed 5MB.");

                // Validate if the new file is different from the existing one
                var uploadedFileHash = await _fileHelper.ComputeFileHashAsync(newProposal.File);
                var existingFileHash = await _googleDriveService.ComputeGoogleDriveFileHashAsync(proposal.Url);

                if (uploadedFileHash != existingFileHash)
                {
                    fileUrl = await _googleDriveService.UploadBudgetProposalFileAsync(newProposal.File);
                    if (string.IsNullOrWhiteSpace(fileUrl))
                        throw new Exception("Failed to upload the file.");

                    await _googleDriveService.SharePdfFileWithUser(fileUrl, user.Email);

                    // Update the proposal's file URL **only if a new file was uploaded**
                    proposal.Url = fileUrl;
                }
            }

            try
            {
                await _dbContext.SaveChangesAsync();
                return proposal.ProposalId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating budget proposal: {ex.Message}");
                return null;
            }
        }
    }
}
