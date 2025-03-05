using AutoMapper;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SubjectDtos;
using LMCM_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.SubjectRepository.SubjectRepository
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly LMCM_DBContext _dbContext;
        private readonly IMapper _mapper;
        public SubjectRepository(LMCM_DBContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<PagedResult<SubjectViewDto>> GetSubjectsAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.Subjects.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                query = query.Where(s => s.SubjectCode.ToLower().Contains(search) ||
                                         s.SubjectName.ToLower().Contains(search));
            }

            int totalCount = await query.CountAsync();

            var items = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            var data = _mapper.Map<List<SubjectViewDto>>(items);

            return new PagedResult<SubjectViewDto>
            {
                Items = data,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }

    }
}
