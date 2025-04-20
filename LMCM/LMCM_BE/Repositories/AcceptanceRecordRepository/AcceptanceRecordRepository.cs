using LMCM_BE.DbContext;
using LMCM_BE.Models;
using LMCM_BE.Shared.Constant;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;

namespace LMCM_BE.Repositories.AcceptanceRecordRepository
{
    public class AcceptanceRecordRepository : IAcceptanceRecordRepository
    {
        private readonly LMCM_DBContext _dbContext;

        public AcceptanceRecordRepository(
            LMCM_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<(List<AcceptanceRecord>, int totalCount)> GetAcceptanceRecordsAsync(bool isHod, Guid userId, string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.AcceptanceRecords.AsQueryable();

            query = query.OrderByDescending(s => s.UpdatedAt)
                .Include(a => a.Contract);

            if (!isHod) query = query.Where(s => s.AuthorId == userId);

            query = query.Where(ar => ar.Status == GenericStatus.Active);

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                query = query.Where(ar => ar.Title.ToLower().Contains(search));
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return (items, totalCount);
        }

        public async Task<bool> CreateAcceptanceRecordAsync(AcceptanceRecord acceptanceRecord)
        {
            await _dbContext.AcceptanceRecords.AddAsync(acceptanceRecord);

            return true;
        }

        public async Task<bool> UpdateAcceptanceRecordAsync(AcceptanceRecord acceptanceRecord)
        {
            _dbContext.Update(acceptanceRecord);
            return true;
        }

        public async Task<AcceptanceRecord?> GetAcceptanceRecordByIdAsync(Guid acceptanceId)
        {
            return await _dbContext.AcceptanceRecords
               .Include(ar => ar.Author)
               .FirstOrDefaultAsync(ar => ar.AcceptanceId == acceptanceId);
        }

        public async Task<AcceptanceRecord?> GetActiveAcceptanceRecordByIdAsync(Guid acceptanceId)
        {
            return await _dbContext.AcceptanceRecords
               .Include(ar => ar.Author)
               .FirstOrDefaultAsync(ar => ar.AcceptanceId == acceptanceId && ar.Status == GenericStatus.Active);
        }

        public async Task<AcceptanceRecord?> GetAcceptanceRecordDetailAsync(Guid acceptanceId)
        {
            return await _dbContext.AcceptanceRecords
                .Include(ar => ar.Contract)
                .ThenInclude(c => c.Contractor)
                .Include(ar => ar.Author)
                .Where(ar => ar.AcceptanceId == acceptanceId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> HasActiveAcceptanceRecordsAsync(Guid contractId)
        {
            return await _dbContext.AcceptanceRecords
                .AnyAsync(p => p.ContractId == contractId && p.Status == GenericStatus.Active);
        }
    }
}
