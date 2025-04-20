using LMCM_BE.DbContext;
using LMCM_BE.Models;
using LMCM_BE.Shared.Constant;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.ContractorRepository
{
    public class ContractorRepository : IContractorRepository
    {
        private readonly LMCM_DBContext _dbContext;

        public ContractorRepository(
            LMCM_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<(List<Contractor>, int totalCount)> GetContractorsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.Contractors
                .Where(c => c.Status == GenericStatus.Active)
                .OrderByDescending(c => c.UpdatedAt)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                query = query.Where(c =>
                    c.ContractorName.ToLower().Contains(search) ||
                    c.PhoneNumber.Contains(search) ||
                    c.Email.ToLower().Contains(search));
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return (items, totalCount);
        }
        public async Task<List<Contractor>> GetContractorsListAsync(string? searchKey)
        {
            var query = _dbContext.Contractors
                .Where(c => c.Status == GenericStatus.Active)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                query = query.Where(c =>
                    c.ContractorName.ToLower().Contains(search) ||
                    c.PhoneNumber.Contains(search) ||
                    c.Email.ToLower().Contains(search));
            }

            return await query
                //.OrderBy(c => c.ContractorName)
                .OrderByDescending(c => c.UpdatedAt)
                .ToListAsync();
        }

        public async Task<bool> CreateContractorAsync(Contractor contractor)
        {
            await _dbContext.Contractors.AddAsync(contractor);

            return true;
        }

        public async Task<bool> UpdateContractorAsync(Contractor contractor)
        {
            _dbContext.Contractors.Update(contractor);
            return true;
        }

        public async Task<Contractor> GetContractorDetailByIdAsync(Guid contractorId)
        {
            var contractor = await _dbContext.Contractors
                .Where(c => c.ContractorId == contractorId && c.Status == GenericStatus.Active)
                .SingleOrDefaultAsync();

            return contractor;
        }
        public async Task<Contractor?> GetActiveContractorByIdAsync(Guid contractorId)
        {
            var contractor = await _dbContext.Contractors
                .Where(c => c.ContractorId == contractorId && c.Status == GenericStatus.Active)
                .SingleOrDefaultAsync();

            return contractor;
        }
        public async Task<Contractor?> GetContractorByIdAsync(Guid contractorId)
        {
            var contractor = await _dbContext.Contractors
                 .Where(c => c.ContractorId == contractorId)
                 .SingleOrDefaultAsync();

            return contractor;
        }
        public async Task<Guid?> CheckContractor(string? taxCode, string? email, string? phoneNumber)
        {
            return await _dbContext.Contractors
                .Where(c => c.Status == GenericStatus.Active)
                .Where(c =>
                    (!string.IsNullOrWhiteSpace(taxCode) && c.TaxCode == taxCode) ||
                    (!string.IsNullOrWhiteSpace(email) && c.Email == email) ||
                    (!string.IsNullOrWhiteSpace(phoneNumber) && c.PhoneNumber == phoneNumber)
                )
                .Select(c => (Guid?)c.ContractorId)
                .FirstOrDefaultAsync();
        }

        public async Task<Guid?> GetDuplicatedContractorIdAsync(string? taxCode, string? employeeCode, string? idCardNumber, string? email)
        {
            return await _dbContext.Contractors
                .Where(c => c.Status == GenericStatus.Active)
                .Where(c =>
                    (!string.IsNullOrWhiteSpace(taxCode) && c.TaxCode == taxCode) ||
                    (!string.IsNullOrWhiteSpace(employeeCode) && c.EmployeeCode == employeeCode) ||
                    (!string.IsNullOrWhiteSpace(idCardNumber) && c.IdCardNumber == idCardNumber) ||
                    (!string.IsNullOrWhiteSpace(email) && c.Email == email)
                )
                .Select(c => (Guid?)c.ContractorId)
                .FirstOrDefaultAsync();
        }
    }
}
