using AutoMapper;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SyllabusDtos;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.SyllabusRepository
{
    public class SyllabusRepository : ISyllabusRepository
    {

        private readonly LMCM_DBContext _dbContext;
        private readonly IMapper _mapper;
        public SyllabusRepository(LMCM_DBContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<PagedResult<SyllabusListViewDto>> GetSyllabusesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.Syllabus.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                query = query.Where(s => s.CourseCode.ToLower().Contains(search) ||
                                         s.CourseNameEnglish.ToLower().Contains(search)||
                                         s.CourseName.ToLower().Contains(search)||
                                         s.Subject.SubjectCode.ToLower().Contains(search)||
                                         s.Subject.SubjectName.ToLower().Contains(search));
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Include(s => s.Subject)
                .ToListAsync();

            var data = _mapper.Map<List<SyllabusListViewDto>>(items);

            return new PagedResult<SyllabusListViewDto>
            {
                Items = data,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }


    }
}
