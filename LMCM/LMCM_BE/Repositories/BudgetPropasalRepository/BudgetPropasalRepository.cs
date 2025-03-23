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
    public class BudgetPropasalRepository : IBudgetPropasalRepository
    {
        private readonly LMCM_DBContext _dbContext;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IMapper _mapper;

        public BudgetPropasalRepository(LMCM_DBContext dbContext, IGoogleDriveService googleDriveService, IMapper mapper)
        {
            _dbContext = dbContext;
            _googleDriveService = googleDriveService;
            _mapper = mapper;
        }
        public async Task<BudgetProposal> CreateBudgetPropasal(BudgetProposalInsertDto propasal)
        {
            // Step 1: Upload contract file to Google Drive (if provided)
            string? fileUrl = null;

            if (propasal.File != null)
            {
                // Check file type
                if (propasal.File.ContentType != "application/pdf")
                {
                    throw new Exception("Only PDF files are allowed.");
                }

                // Check file size (5MB = 5 * 1024 * 1024 bytes)
                if (propasal.File.Length > 5 * 1024 * 1024)
                {
                    throw new Exception("File size must not exceed 5MB.");
                }

                fileUrl = await _googleDriveService.UploadBudgetPropasalFileAsync(propasal.File);
            }

            // Step 2: Create Contract object
            var newPropasal = _mapper.Map<BudgetProposal>(propasal);

            newPropasal.ProposalId = Guid.NewGuid();
            newPropasal.Url = fileUrl;
            newPropasal.Status = "Active";
            newPropasal.CreatedAt = DateTime.UtcNow;
            newPropasal.UpdatedAt = DateTime.UtcNow;

            // Step 3: Save to database
            _dbContext.BudgetProposals.Add(newPropasal);
            await _dbContext.SaveChangesAsync();

            return newPropasal;
        }

        public async Task<BudgetPropasalDetailDto> GetBudgetPropasalById(Guid? propasalId)
        {
            if (propasalId == null)
                throw new ArgumentNullException(nameof(propasalId), "propasal ID cannot be null.");
            var budgetProposal = await _dbContext.BudgetProposals
                .AsNoTracking()
                .Where(s => s.ProposalId == propasalId)
                .SingleOrDefaultAsync();

            if (budgetProposal == null)
                throw new KeyNotFoundException($"No syllabus found with ID: {propasalId}");
            var budgetPropasalDto = _mapper.Map<BudgetPropasalDetailDto>(budgetProposal);
            return budgetPropasalDto;
        }

        public async Task<PagedResult<BudgetProposalListDto>> GetBudgetPropasalsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
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

        public async Task<Guid?> UpdateBudgetPropasalAsync(Guid propasalId, BudgetProposalUpdateDto newPropasal)
        {
            if (propasalId == null)
                throw new ArgumentNullException(nameof(propasalId), "propasal id cannot be null.");

            if (newPropasal == null)
                throw new ArgumentNullException(nameof(newPropasal), "New propasal data cannot be null.");

            var propasal = await _dbContext.BudgetProposals
                    .Include(lm => lm.Author)
                    .FirstOrDefaultAsync(lm => lm.ProposalId == propasalId);

            if (propasal == null)
                throw new ArgumentNullException(nameof(propasal), "propasal data not found.");
            if(newPropasal.AuthorId!=propasal.AuthorId)
                throw new ArgumentNullException(nameof(newPropasal.AuthorId), "User is not authorized to update this material.");

            string? fileUrl = null;

            if (newPropasal.File != null)
            {
                // Check file type
                if (newPropasal.File.ContentType != "application/pdf")
                {
                    throw new Exception("Only PDF files are allowed.");
                }

                // Check file size (5MB = 5 * 1024 * 1024 bytes)
                if (newPropasal.File.Length > 5 * 1024 * 1024)
                {
                    throw new Exception("File size must not exceed 5MB.");
                }

                fileUrl = await _googleDriveService.UploadBudgetPropasalFileAsync(newPropasal.File);
            }

            // Use AutoMapper to update existing entity
            _mapper.Map(newPropasal, propasal);
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
