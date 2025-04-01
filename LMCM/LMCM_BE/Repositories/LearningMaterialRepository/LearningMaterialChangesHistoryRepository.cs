using AutoMapper;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.LearningMaterialDtos;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SyllabusDtos;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Models;
using LMCM_BE.Repositories.UserRepositoriy;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;

namespace LMCM_BE.Repositories.LearningMaterialRepository
{
    public class LearningMaterialChangesHistoryRepository : ILearningMaterialChangesHistoryRepository
    {
        private readonly LMCM_DBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepositoriy;
        public LearningMaterialChangesHistoryRepository(LMCM_DBContext dbContext, IMapper mapper,IUserRepository userRepository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userRepositoriy = userRepository;
        }
        public async Task<PagedResult<ChangesHistoryListDto>> GetChangesHistoriesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
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

            var data = _mapper.Map<List<ChangesHistoryListDto>>(items);

            return new PagedResult<ChangesHistoryListDto>
            {
                Items = data,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }
        public async Task<bool> CreateLearningMaterialChangesHistoryAsync(CreateLearningMaterialChangesHistoryDto historyDto)
        {
            if (historyDto == null)
            {
                throw new ArgumentNullException(nameof(historyDto));
            }
            try
            {
                UserProfileResponseDto user = await _userRepositoriy.GetProfileFromCookie();
                if (user == null || string.IsNullOrEmpty(user.Email))
                    throw new Exception("User not found");

                var history = _mapper.Map<LearningMaterialChangesHistory>(historyDto);
                history.UserId= user.Id;    
                history.HistoryId = Guid.NewGuid();
                history.Status = "Active";
                await _dbContext.LearningMaterialChangesHistories.AddAsync(history);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<PagedResult<ChangesHistoryOfSubjectDto>> GetLearningMaterialChangesHistoriesOfSubjectAsync(
     Guid? subjectId, string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            if (subjectId == Guid.Empty)
                throw new ArgumentException("ID môn học không được để trống.", nameof(subjectId));

            var siblingSyllabusIds = await _dbContext.Syllabus
                .Where(s => s.SubjectId == subjectId)
                .Select(s => s.SyllabusId)
                .ToListAsync();

            var historyChain = await _dbContext.LearningMaterialChangesHistories
                .Include(h => h.User)
                .Include(h => h.Contract)
                .Include(h => h.Syllabus)
                .Where(h => siblingSyllabusIds.Contains(h.SyllabusId))
                .OrderByDescending(h => h.Syllabus.UpdatedAt ?? h.Syllabus.CreatedAt)  // Newest syllabus first
                .ThenByDescending(h => h.CompletionDate ?? DateTime.MinValue)  // Then by CompletionDate if not null
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                historyChain = historyChain.Where(h =>
                    (h.CourseCode != null && h.CourseCode.ToLower().Contains(search)) ||
                    (h.User.UserName.ToLower().Contains(search))
                ).ToList();
            }

            int totalCount = historyChain.Count;

            // Step 5: Paginate results
            var paginatedItems = historyChain
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var data = _mapper.Map<List<ChangesHistoryOfSubjectDto>>(paginatedItems);

            return new PagedResult<ChangesHistoryOfSubjectDto>
            {
                Items = data,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
