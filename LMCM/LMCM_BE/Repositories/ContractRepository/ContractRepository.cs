using AutoMapper;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.ContractDtos;
using LMCM_BE.Models;
using LMCM_BE.Services.GoogleDriveService;
using Microsoft.EntityFrameworkCore;
using System;

namespace LMCM_BE.Repositories.ContractRepository
{
    public class ContractRepository : IContractRepository
    {
        private readonly LMCM_DBContext _context;
        private readonly GoogleDriveService _googleDriveService;
        private readonly IMapper _mapper;

        public ContractRepository(LMCM_DBContext context, GoogleDriveService googleDriveService, IMapper mapper)
        {
            _context = context;
            _googleDriveService = googleDriveService;
            _mapper = mapper;
        }

        public async Task<Contract> CreateContract(ContractInsertDto contractDto)
        {
            // Step 1: Upload contract file to Google Drive (if provided)
            string? fileUrl = null;
            if (contractDto.File != null)
            {
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
            _context.Contracts.Add(newContract);
            await _context.SaveChangesAsync();

            return newContract;
        }
        public async Task<Contract?> GetContractByIdAsync(Guid contractId)
        {
            return await _context.Contracts
                .FirstOrDefaultAsync(c => c.ContractId == contractId && c.Status == "Active");
        }

    }
}
