using AutoMapper;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.BudgetProposalDtos;
using LMCM_BE.Models;
using LMCM_BE.Services.GoogleDriveService;

namespace LMCM_BE.Repositories.BudgetPropasalRepository
{
    public class BudgetPropasalRepository : IBudgetPropasalRepository
    {
        private readonly LMCM_DBContext _context;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IMapper _mapper;

        public BudgetPropasalRepository(LMCM_DBContext context, IGoogleDriveService googleDriveService, IMapper mapper)
        {
            _context = context;
            _googleDriveService = googleDriveService;
            _mapper = mapper;
        }
        public async Task<BudgetProposal> CreateBudgetPropasal(BudgetProposalInsertDto propasal)
        {
            // Step 1: Upload contract file to Google Drive (if provided)
            string? fileUrl = null;

            if (propasal.File != null)
            {
                Console.WriteLine("ok");
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
            _context.BudgetProposals.Add(newPropasal);
            await _context.SaveChangesAsync();

            return newPropasal;
        }
    }
}
