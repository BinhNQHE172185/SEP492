using AutoMapper;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.AcceptanceRecordDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.ContractRepository;
using LMCM_BE.Repositories.UserRepositoriy;
using LMCM_BE.Services.GoogleDriveService;
using LMCM_BE.Utilities;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.AcceptanceRecordRepository
{
    public class AcceptanceRecordRepository : IAcceptanceRecordRepository
    {
        private readonly LMCM_DBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IContractRepository _contractRepository;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IFileHelper _fileHelper;
        private readonly IUserRepository _userRepository;

        public AcceptanceRecordRepository(
            LMCM_DBContext dbContext,
            IMapper mapper,
            IContractRepository contractRepository,
            IGoogleDriveService googleDriveService,
            IFileHelper fileHelper,
            IUserRepository userRepository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _contractRepository = contractRepository;
            _googleDriveService = googleDriveService;
            _fileHelper = fileHelper;
            _userRepository = userRepository;
        }

        public async Task<PagedResult<AcceptanceRecordListDto>> GetAcceptanceRecordsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.AcceptanceRecords
                .Where(ar => ar.Status == "Active")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                query = query.Where(ar => ar.Title.ToLower().Contains(search));
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            var data = _mapper.Map<List<AcceptanceRecordListDto>>(items);

            return new PagedResult<AcceptanceRecordListDto>
            {
                Items = data,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<AcceptanceRecordDetailDto> CreateAcceptanceRecordAsync(AcceptanceRecordCreateDto dto)
        {
            // Bước 1: Tải tệp lên Google Drive nếu có
            string? fileUrl = null;
            if (dto.File != null)
            {
                // Kiểm tra định dạng tệp (chỉ cho phép PDF)
                if (dto.File.ContentType != "application/pdf")
                {
                    throw new Exception("Chỉ cho phép tải lên tệp PDF.");
                }

                // Kiểm tra kích thước tệp (tối đa 5MB)
                if (dto.File.Length > 5 * 1024 * 1024)
                {
                    throw new Exception("Kích thước tệp không được vượt quá 5MB.");
                }

                fileUrl = await _googleDriveService.UploadAcceptanceRecordFileAsync(dto.File);

                if (string.IsNullOrWhiteSpace(fileUrl))
                {
                    throw new Exception("Tải tệp lên thất bại. Vui lòng thử lại.");
                }
                else
                {
                    var user = await _userRepository.GetProfile(dto.AuthorId.ToString());
                    if (user == null||string.IsNullOrEmpty(user.Email))
                        throw new Exception("Không tìm thấy email");
                    await _googleDriveService.SharePdfFileWithUser(fileUrl, user.Email);
                }
            }
            else
            {
                throw new Exception("Vui lòng tải lên tệp biên bản nghiệm thu.");
            }

            // Bước 2: Chuyển đổi DTO sang entity
            var acceptanceRecord = _mapper.Map<AcceptanceRecord>(dto);
            acceptanceRecord.AcceptanceId = Guid.NewGuid();
            acceptanceRecord.Url = fileUrl; // Lưu URL của tệp đã tải lên
            acceptanceRecord.Status = "Active";
            acceptanceRecord.CreatedAt = DateTime.UtcNow;
            acceptanceRecord.UpdatedAt = DateTime.UtcNow;

            // Bước 3: Lưu vào cơ sở dữ liệu
            await _dbContext.AcceptanceRecords.AddAsync(acceptanceRecord);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<AcceptanceRecordDetailDto>(acceptanceRecord);
        }

        public async Task<Guid?> UpdateAcceptanceRecordAsync(Guid acceptanceId, AcceptanceRecordUpdateDto dto)
        {
            if (acceptanceId == Guid.Empty)
                throw new ArgumentException("ID biên bản nghiệm thu không được để trống.", nameof(acceptanceId));

            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Dữ liệu cập nhật không được để trống.");

            var acceptanceRecord = await _dbContext.AcceptanceRecords
                .Include(ar => ar.Author)
                .FirstOrDefaultAsync(ar => ar.AcceptanceId == acceptanceId && ar.Status == "Active");

            if (acceptanceRecord == null)
                throw new KeyNotFoundException("Không tìm thấy biên bản nghiệm thu.");

            if (dto.AuthorId != acceptanceRecord.AuthorId)
                throw new UnauthorizedAccessException("Bạn không có quyền cập nhật biên bản nghiệm thu này.");

            string? fileUrl = null;

            if (dto.File != null)
            {
                // Kiểm tra định dạng tệp
                if (dto.File.ContentType != "application/pdf")
                    throw new InvalidOperationException("Chỉ cho phép tải lên tệp PDF.");

                // Giới hạn kích thước tệp (5MB)
                const int maxFileSize = 5 * 1024 * 1024;
                if (dto.File.Length > maxFileSize)
                    throw new InvalidOperationException("Kích thước tệp không được vượt quá 5MB.");

                // Kiểm tra nếu tệp mới khác tệp cũ trước khi tải lên
                var uploadedFileHash = await _fileHelper.ComputeFileHashAsync(dto.File);
                var existingFileHash = await _googleDriveService.ComputeGoogleDriveFileHashAsync(acceptanceRecord.Url);

                if (uploadedFileHash != existingFileHash)
                {
                    fileUrl = await _googleDriveService.UploadAcceptanceRecordFileAsync(dto.File);
                    if (string.IsNullOrWhiteSpace(fileUrl))
                    {
                        throw new Exception("Tải tệp lên thất bại. Vui lòng thử lại.");
                    }
                    else
                    {
                        var user = await _userRepository.GetProfile(dto.AuthorId.ToString());
                        if (user == null || string.IsNullOrEmpty(user.Email))
                            throw new Exception("Không tìm thấy email");
                        await _googleDriveService.SharePdfFileWithUser(fileUrl, user.Email);
                    }
                }
            }

            _mapper.Map(dto, acceptanceRecord);
            acceptanceRecord.UpdatedAt = DateTime.UtcNow;

            if (fileUrl != null)
                acceptanceRecord.Url = fileUrl;

            await _dbContext.SaveChangesAsync();
            return acceptanceRecord.AcceptanceId;
        }

        public async Task<bool> SoftDeleteAcceptanceRecordAsync(Guid acceptanceId)
        {
            var acceptanceRecord = await _dbContext.AcceptanceRecords
                .FirstOrDefaultAsync(ar => ar.AcceptanceId == acceptanceId && ar.Status == "Active");

            if (acceptanceRecord == null)
                throw new KeyNotFoundException("Không tìm thấy biên bản nghiệm thu hoặc đã bị xóa.");

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                acceptanceRecord.Status = "Inactive";
                acceptanceRecord.UpdatedAt = DateTime.UtcNow;
                _dbContext.AcceptanceRecords.Update(acceptanceRecord);

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

        public async Task<AcceptanceRecordDetailDto> GetAcceptanceRecordDetailAsync(Guid acceptanceId)
        {
            var acceptanceRecord = await _dbContext.AcceptanceRecords
                .Include(ar => ar.Contract)
                .Include(ar => ar.Author)
                .Where(ar => ar.AcceptanceId == acceptanceId && ar.Status == "Active")
                .FirstOrDefaultAsync();

            if (acceptanceRecord == null)
                throw new KeyNotFoundException("Không tìm thấy biên bản nghiệm thu.");

            var acceptanceRecordDto = _mapper.Map<AcceptanceRecordDetailDto>(acceptanceRecord);

            // Check if there's a file URL and fetch the file
            //if (!string.IsNullOrEmpty(acceptanceRecord.Url))
            //{
            //    var fileId =await _fileHelper.ExtractFileIdFromUrl(acceptanceRecord.Url);
            //    var (fileContent, fileName) = await _googleDriveService.FetchFileAsync(fileId);

            //    if (fileContent != null)
            //    {
            //        acceptanceRecordDto.FileContent = fileContent;
            //        acceptanceRecordDto.FileName = fileName;
            //    }

            //}

            return acceptanceRecordDto;
        }

        public async Task<bool> HasActiveAcceptanceRecordsAsync(Guid contractId)
        {
            return await _dbContext.AcceptanceRecords
                .AnyAsync(p => p.ContractId == contractId && p.Status == "Active");
        }
    }
}
