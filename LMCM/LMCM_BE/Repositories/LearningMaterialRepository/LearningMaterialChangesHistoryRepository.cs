using AutoMapper;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;

using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.LearningMaterialRepository
{
    public class LearningMaterialChangesHistoryRepository : ILearningMaterialChangesHistoryRepository
    {
        private readonly LMCM_DBContext _dbContext;
        private readonly IMapper _mapper;
        public LearningMaterialChangesHistoryRepository(LMCM_DBContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<PagedResult<ChangesHistoryListDto>> GetChangesHistoriesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.LearningMaterialChangesHistories.AsQueryable();

            query = query.Where(s=>s.Status != "Inactive");

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
                .ToListAsync();

            var data = _mapper.Map<List<ChangesHistoryListDto>>(items);

            return new PagedResult<ChangesHistoryListDto>
            {
                Items = data,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
