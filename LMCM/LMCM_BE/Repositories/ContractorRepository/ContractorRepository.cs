using AutoMapper;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.ContractorDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.ContractRepository;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.ContractorRepository
{
    public class ContractorRepository : IContractorRepository
    {
        private readonly LMCM_DBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IContractRepository _contractRepository;

        public ContractorRepository(
            LMCM_DBContext dbContext,
            IMapper mapper,
            IContractRepository contractRepository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _contractRepository = contractRepository;
        }

        public async Task<PagedResult<ContractorListDto>> GetContractorsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.Contractors
                .Where(c => c.Status == "Active")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                query = query.Where(c => c.ContractorName.ToLower().Contains(search) ||
                                         c.PhoneNumber.Contains(search) ||
                                         c.Email.ToLower().Contains(search));
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            var data = _mapper.Map<List<ContractorListDto>>(items);

            return new PagedResult<ContractorListDto>
            {
                Items = data,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<List<ContractorListDto>> GetContractorsListAsync()
        {
            var query = _dbContext.Contractors
                .Where(c => c.Status == "Active")
                .AsQueryable();

            var items = await query.ToListAsync();

            var data = _mapper.Map<List<ContractorListDto>>(items);

            return data;
        }

        public async Task<ContractorDetailDto> CreateContractorAsync(ContractorCreateDto dto)
        {
            var contractor = _mapper.Map<Contractor>(dto);
            contractor.ContractorId = Guid.NewGuid();
            contractor.Status = "Active";
            contractor.CreatedAt = DateTime.UtcNow;
            contractor.UpdatedAt = DateTime.UtcNow;

            await _dbContext.Contractors.AddAsync(contractor);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<ContractorDetailDto>(contractor);
        }

        public async Task<Guid?> UpdateContractorAsync(Guid contractorId, ContractorUpdateDto dto)
        {
            var contractor = await _dbContext.Contractors
                .FirstOrDefaultAsync(c => c.ContractorId == contractorId && c.Status == "Active");

            if (contractor == null)
                throw new KeyNotFoundException("Không tìm thấy nhà thầu.");

            _mapper.Map(dto, contractor);
            contractor.UpdatedAt = DateTime.UtcNow;

            _dbContext.Contractors.Update(contractor);
            await _dbContext.SaveChangesAsync();

            return contractor.ContractorId;
        }

        public async Task<bool> SoftDeleteContractorAsync(Guid contractorId)
        {
            var contractor = await _dbContext.Contractors
                .FirstOrDefaultAsync(c => c.ContractorId == contractorId && c.Status == "Active");

            if (contractor == null)
                throw new KeyNotFoundException("Không tìm thấy nhà thầu hoặc đã bị xóa.");

            if (await _contractRepository.HasActiveContractsAsync(contractorId))
                throw new InvalidOperationException("Không thể xóa nhà thầu khi vẫn có hợp đồng đang hoạt động.");

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                contractor.Status = "Inactive";
                contractor.UpdatedAt = DateTime.UtcNow;
                _dbContext.Contractors.Update(contractor);

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

        public async Task<ContractorDetailDto> GetContractorDetailAsync(Guid contractorId)
        {
            var contractor = await _dbContext.Contractors
                .Include(c => c.Contracts)
                .Where(c => c.ContractorId == contractorId && c.Status == "Active")
                .FirstOrDefaultAsync();

            if (contractor == null)
                throw new KeyNotFoundException("Không tìm thấy nhà thầu.");

            return _mapper.Map<ContractorDetailDto>(contractor);
        }
    }
}
