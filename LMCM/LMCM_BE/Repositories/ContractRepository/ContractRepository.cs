using AutoMapper;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.BudgetProposalDtos;
using LMCM_BE.DTOs.ContractDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
using LMCM_BE.Services.GoogleDriveService;
using Microsoft.EntityFrameworkCore;
using System;

namespace LMCM_BE.Repositories.ContractRepository
{
    public class ContractRepository : IContractRepository
    {
        private readonly LMCM_DBContext _dbContext;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IMapper _mapper;

        public ContractRepository(LMCM_DBContext dbContext, IGoogleDriveService googleDriveService, IMapper mapper)
        {
            _dbContext = dbContext;
            _googleDriveService = googleDriveService;
            _mapper = mapper;
        }

        public async Task<Contract> CreateContract(ContractInsertDto contractDto)
        {
            // Step 1: Upload contract file to Google Drive (if provided)
            string? fileUrl = null;
            if (contractDto.File != null)
            {
                // Check file type
                if (contractDto.File.ContentType != "application/pdf")
                {
                    throw new Exception("Only PDF files are allowed.");
                }

                // Check file size (5MB = 5 * 1024 * 1024 bytes)
                if (contractDto.File.Length > 5 * 1024 * 1024)
                {
                    throw new Exception("File size must not exceed 5MB.");
                }

                fileUrl = await _googleDriveService.UploadContractFileAsync(contractDto.File);
            }

            // Step 2: Create Contract object
            var newContract = _mapper.Map<Contract>(contractDto);

            newContract.ContractId = Guid.NewGuid();
            newContract.Url = fileUrl;
            newContract.Status = "Active";
            newContract.CreatedAt = DateTime.UtcNow;
            newContract.UpdatedAt = DateTime.UtcNow;

            // Step 3: Save to database
            _dbContext.Contracts.Add(newContract);
            await _dbContext.SaveChangesAsync();

            return newContract;
        }
        public async Task<Contract?> GetContractByIdAsync(Guid contractId)
        {
            return await _dbContext.Contracts
                .FirstOrDefaultAsync(c => c.ContractId == contractId && c.Status == "Active");
        }

        public async Task<PagedResult<ContractListDto>> GetContractsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.Contracts.AsQueryable();

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
                .Include(s=>s.Proposal)
                .ToListAsync();

            var data = _mapper.Map<List<ContractListDto>>(items);

            return new PagedResult<ContractListDto>
            {
                Items = data,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
