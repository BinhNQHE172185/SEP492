using LMCM_BE.DbContext;
using LMCM_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.LearningMaterialChangesHistoryRepository
{
    public class LearningMaterialChangesHistoryRepository : ILearningMaterialChangesHistoryRepository
    {
        private readonly LMCM_DBContext _dbContext;
        public LearningMaterialChangesHistoryRepository(LMCM_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<(List<LearningMaterialChangesHistory>, int totalCount)> GetChangesHistoriesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.LearningMaterialChangesHistories.AsQueryable();

            query = query.Where(s => s.Status != "Inactive");

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                query = query.Where(s => s.CourseCode.ToLower().Contains(search) ||
                                         s.User.UserName.ToLower().Contains(search));
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Include(s => s.User)
                .Include(s => s.Contract)
                .Include(s => s.Syllabus)
                .ToListAsync();

            return (items, totalCount);
        }
        public async Task<bool> CreateLearningMaterialChangesHistoryAsync(LearningMaterialChangesHistory history)
        {
            await _dbContext.LearningMaterialChangesHistories.AddAsync(history);
            return true;
        }
        public async Task<(List<LearningMaterialChangesHistory>, int totalCount)> GetLearningMaterialChangesHistoriesOfSubjectAsync(
    List<Syllabus> syllabuses, string? searchKey, int pageIndex = 1, int pageSize = 10)
        {

            var historyChain = await _dbContext.LearningMaterialChangesHistories
                .Include(h => h.User)
                .Include(h => h.Contract)
                .Include(h => h.Syllabus)
                .Where(h => syllabuses.Contains(h.Syllabus))
                .OrderByDescending(h => h.Syllabus.UpdatedAt ?? h.Syllabus.CreatedAt)  // Newest syllabus first
                .ThenByDescending(h => h.CompletionDate ?? DateTime.MinValue)  // Then by CompletionDate if not null
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                historyChain = historyChain.Where(h =>
                    h.CourseCode != null && h.CourseCode.ToLower().Contains(search) ||
                    h.User.UserName.ToLower().Contains(search)
                ).ToList();
            }

            int totalCount = historyChain.Count;

            var paginatedItems = historyChain
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return (paginatedItems, totalCount);
        }
        public async Task<bool> UpdateLearningMaterialChangesHistoryAsync(LearningMaterialChangesHistory history)
        {
            _dbContext.LearningMaterialChangesHistories.Update(history);
            return true ;
        }
        public async Task<LearningMaterialChangesHistory?> getHistoryOfChangeDetail(Guid id)
        {
            return await _dbContext.LearningMaterialChangesHistories
                            .Where(h => h.HistoryId == id && h.Status == "Active")
                            .Include(h => h.Syllabus)
                            .Include(h => h.Contract)
                            .FirstOrDefaultAsync();;
        }
        public async Task<LearningMaterialChangesHistory?> GetActiveHistoryByIdAsync(Guid historyId)
        {
            return await _dbContext.LearningMaterialChangesHistories
                .FirstOrDefaultAsync(h => h.HistoryId == historyId && h.Status == "Active");
        }

        public async Task<List<LearningMaterialChangesHistory>> GetAllWithCompletionDateAsync()
        {
            return await _dbContext.LearningMaterialChangesHistories
                .Where(h => h.CompletionDate.HasValue)
                .ToListAsync();
        }
    }
}
