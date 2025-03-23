using AutoMapper;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.BudgetProposalDtos;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SyllabusDtos;
using LMCM_BE.Models;
using LMCM_BE.Services.GoogleDriveService;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.BudgetPropasalRepository
{
    public class BudgetProposalRepository : IBudgetProposalRepository
    {
        private readonly LMCM_DBContext _dbContext;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IMapper _mapper;

        public BudgetProposalRepository(LMCM_DBContext dbContext, IGoogleDriveService googleDriveService, IMapper mapper)
        {
            _dbContext = dbContext;
            _googleDriveService = googleDriveService;
            _mapper = mapper;
        }
        public async Task<BudgetProposal> CreateBudgetProposal(BudgetProposalInsertDto proposal)
        {
            if (proposal == null)
            {
                throw new ArgumentNullException(nameof(proposal), "Proposal data is required.");
            }
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
            }

            // Step 2: Create Contract object
            var newProposal = _mapper.Map<BudgetProposal>(proposal);

            newProposal.ProposalId = Guid.NewGuid();
            newProposal.Url = fileUrl;
            newProposal.Status = "Active";
            newProposal.CreatedAt = DateTime.UtcNow;
            newProposal.UpdatedAt = DateTime.UtcNow;

            // Step 3: Save to database
            _dbContext.BudgetProposals.Add(newProposal);
            await _dbContext.SaveChangesAsync();

            return newProposal;
        }

        public async Task<BudgetProposalDetailDto> GetBudgetProposalById(Guid proposalId)
        {
            if (proposalId == Guid.Empty)
                throw new ArgumentException("Proposal ID cannot be empty.", nameof(proposalId));

            var budgetProposal = await _dbContext.BudgetProposals
                .AsNoTracking()
                .Where(s => s.ProposalId == proposalId)
                .SingleOrDefaultAsync();

            if (budgetProposal == null)
                throw new KeyNotFoundException($"No budget proposal found with ID: {proposalId}");

            var budgetProposalDto = _mapper.Map<BudgetProposalDetailDto>(budgetProposal);

            // Check if there's a file URL and fetch the file
            if (!string.IsNullOrEmpty(budgetProposal.Url))
            {
                using var httpClient = new HttpClient();
                var fileBytes = await httpClient.GetByteArrayAsync(budgetProposal.Url);

                budgetProposalDto.FileContent = fileBytes; // Add the file as a byte array
                budgetProposalDto.FileName = $"BudgetProposal_{proposalId}.pdf"; 
            }

            return budgetProposalDto;
        }

        public async Task<PagedResult<BudgetProposalListDto>> GetBudgetProposalsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.BudgetProposals.AsQueryable();

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

        public async Task<Guid?> UpdateBudgetProposalAsync(Guid proposalId, BudgetProposalUpdateDto newProposal)
        {
            if (proposalId == Guid.Empty)
                throw new ArgumentNullException(nameof(proposalId), "proposal id cannot be null.");

            if (newProposal == null)
                throw new ArgumentNullException(nameof(newProposal), "New proposal data cannot be null.");

            var propasal = await _dbContext.BudgetProposals
                    .Include(lm => lm.Author)
                    .FirstOrDefaultAsync(lm => lm.ProposalId == proposalId);

            if (propasal == null)
                throw new ArgumentNullException(nameof(propasal), "propasal data not found.");
            if(newProposal.AuthorId!=propasal.AuthorId)
                throw new ArgumentNullException(nameof(newProposal.AuthorId), "User is not authorized to update this proposal.");

            string? fileUrl = null;

            if (newProposal.File != null)
            {
                // Check file type
                if (newProposal.File.ContentType != "application/pdf")
                {
                    throw new Exception("Only PDF files are allowed.");
                }

                // Check file size (5MB = 5 * 1024 * 1024 bytes)
                if (newProposal.File.Length > 5 * 1024 * 1024)
                {
                    throw new Exception("File size must not exceed 5MB.");
                }

                fileUrl = await _googleDriveService.UploadBudgetProposalFileAsync(newProposal.File);
            }

            // Use AutoMapper to update existing entity
            _mapper.Map(newProposal, propasal);
            propasal.UpdatedAt = DateTime.UtcNow;
            if(fileUrl != null)propasal.Url = fileUrl;

            try
            {
                await _dbContext.SaveChangesAsync();
                return propasal.ProposalId;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
