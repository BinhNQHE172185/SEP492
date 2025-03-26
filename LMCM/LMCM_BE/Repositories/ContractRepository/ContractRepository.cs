using AutoMapper;
using AutoMapper.QueryableExtensions;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.ContractDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.UserRepositoriy;
using LMCM_BE.Services.GoogleDriveService;
using LMCM_BE.Utilities;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.ContractRepository
{
    public class ContractRepository : IContractRepository
    {
        private readonly LMCM_DBContext _dbContext;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IMapper _mapper;
        private readonly IFileHelper _fileHelper;
        IUserRepository _userRepository;

        public ContractRepository(LMCM_DBContext dbContext, IGoogleDriveService googleDriveService, IMapper mapper, IFileHelper fileHelper, IUserRepository userRepository)
        {
            _dbContext = dbContext;
            _googleDriveService = googleDriveService;
            _mapper = mapper;
            _fileHelper = fileHelper;
            _userRepository = userRepository;
        }

        public async Task<bool> CreateContract(ContractInsertDto contractDto)
        {
            UserProfileResponseDto user = await _userRepository.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new Exception("User not found");
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

            newContract.ContractId = Guid.NewGuid();
            newContract.Url = fileUrl;
            newContract.Status = "Active";
            newContract.CreatedAt = DateTime.UtcNow;
            newContract.UpdatedAt = DateTime.UtcNow;

            // Step 3: Save to database
            _dbContext.Contracts.Add(newContract);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<ContractDetailDto> GetContractByIdAsync(Guid contractId)
        {
            if (contractId == Guid.Empty)
                throw new ArgumentException("Contract ID cannot be empty.", nameof(contractId));

            var contract = await _dbContext.Contracts
                .AsNoTracking()
                .Where(s => s.ContractId == contractId)
                .SingleOrDefaultAsync();

            if (contract == null)
                throw new KeyNotFoundException($"No contract found with ID: {contractId}");

            UserProfileResponseDto user = await _userRepository.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new Exception("User not found");
            if (user.Id != contract.AuthorId && user.Roles.Contains("Staff"))
                throw new UnauthorizedAccessException("User is not authorized to view this contract.");

            var contractDto = _mapper.Map<ContractDetailDto>(contract);
            contractDto.DownloadUrl = await _googleDriveService.GetDownloadUrl(contract.Url);

            // Check if there's a file URL and fetch the file
            //if (!string.IsNullOrEmpty(contract.Url))
            //{
            //    var fileId = await _fileHelper.ExtractFileIdFromUrl(contract.Url);
            //    var (fileContent, fileName) = await _googleDriveService.FetchFileAsync(fileId);

            //    if (fileContent != null)
            //    {
            //        contractDto.FileContent = fileContent;
            //        contractDto.FileName = fileName;
            //    }
            //}

            return contractDto;
        }

        public async Task<PagedResult<ContractListDto>> GetContractsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.Contracts.AsQueryable();

            UserProfileResponseDto user = await _userRepository.GetProfileFromCookie();
            if (user != null && !user.Roles.Contains("Admin")) query = query.Where(s => s.AuthorId == user.Id);

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
                .Include(s => s.Proposal)
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

        public async Task<bool> HasActiveConntractsAsync(Guid proposalId)
        {
            return await _dbContext.Contracts
                  .AnyAsync(p => p.ProposalId == proposalId && p.Status == "Active");
        }

        public async Task<bool> HasActiveContractsAsync(Guid contractorId)
        {
            return await _dbContext.Contracts
                .AnyAsync(p => p.ContractorId == contractorId && p.Status == "Active");
        }

        public async Task<bool> SoftDeleteContractAsync(Guid contractId)
        {
            var contract = await _dbContext.Contracts
                .FirstOrDefaultAsync(ar => ar.ContractId == contractId && ar.Status == "Active");

            if (contract == null)
                throw new KeyNotFoundException("Data not found.");

            UserProfileResponseDto user = await _userRepository.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new Exception("User not found");
            if (user.Id != contract.AuthorId)
                throw new UnauthorizedAccessException("User is not authorized to delete this contract.");

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                contract.Status = "Inactive";
                contract.UpdatedAt = DateTime.UtcNow;
                _dbContext.Contracts.Update(contract);

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

        public async Task<Guid?> UpdateContractAsync(Guid contractId, ContractUpdateDto newContract)
        {
            if (contractId == Guid.Empty)
                throw new ArgumentException("Contract ID cannot be empty.", nameof(contractId));

            if (newContract == null)
                throw new ArgumentNullException(nameof(newContract), "New contract data cannot be null.");

            var contract = await _dbContext.Contracts
                .Include(lm => lm.Author)
                .Include(lm => lm.Proposal)
                .Include(lm => lm.Contractor)
                .FirstOrDefaultAsync(lm => lm.ContractId == contractId);

            if (contract == null)
                throw new KeyNotFoundException($"No contract found with ID: {contractId}");

            UserProfileResponseDto user = await _userRepository.GetProfileFromCookie();
            if (user == null || string.IsNullOrEmpty(user.Email))
                throw new Exception("User not found");
            if (user.Id != contract.AuthorId)
                throw new UnauthorizedAccessException("User is not authorized to update this contract.");

            // Update contract fields (excluding file)
            _mapper.Map(newContract, contract);
            contract.UpdatedAt = DateTime.UtcNow;

            string? fileUrl = null;

            if (newContract.File != null)
            {
                // Validate file type
                if (newContract.File.ContentType != "application/pdf")
                    throw new InvalidOperationException("Only PDF files are allowed.");

                // Validate file size (5MB limit)
                const int maxFileSize = 5 * 1024 * 1024;
                if (newContract.File.Length > maxFileSize)
                    throw new InvalidOperationException("File size must not exceed 5MB.");

                // Validate if the new file is different from the existing one
                var uploadedFileHash = await _fileHelper.ComputeFileHashAsync(newContract.File);
                var existingFileHash = await _googleDriveService.ComputeGoogleDriveFileHashAsync(contract.Url);

                if (uploadedFileHash != existingFileHash)
                {
                    fileUrl = await _googleDriveService.UploadContractFileAsync(newContract.File);
                    if (string.IsNullOrWhiteSpace(fileUrl))
                        throw new Exception("Failed to upload the file.");

                    await _googleDriveService.SharePdfFileWithUser(fileUrl, user.Email);

                    // Update the contract's file URL **only if a new file was uploaded**
                    contract.Url = fileUrl;
                }
            }

            try
            {
                await _dbContext.SaveChangesAsync();
                return contract.ContractId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating Contract: {ex.Message}");
                return null;
            }
        }

    }
}
