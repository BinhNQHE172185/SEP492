using AutoMapper;
using AutoMapper.QueryableExtensions;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SyllabusDtos;
using LMCM_BE.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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

        public async Task<bool> DeleteSyllabusAsync(Guid id)
        {
            var syllabus = await _dbContext.Syllabus.FindAsync(id);
            if (syllabus == null)
                return false; // Syllabus not found

            syllabus.Status = "Inactive";
            syllabus.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }


        public async Task<PagedResult<SyllabusListViewDto>> GetSyllabusesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var query = _dbContext.Syllabus.AsQueryable();

            query = query.OrderBy(s => s.CourseCode);

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                query = query.Where(s => s.CourseCode.ToLower().Contains(search) ||
                                         s.CourseNameEnglish.ToLower().Contains(search) ||
                                         s.CourseName.ToLower().Contains(search));
            }

            query = query.Where(s => s.Status != "Inactive");

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
        public async Task<PagedResult<SyllabusListViewDto>> GetSyllabusChangeHistoriesAsync(
            Guid? syllabusId, string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            if (syllabusId == null)
                throw new ArgumentNullException(nameof(syllabusId));

            // Step 1: Get the subject ID from the given syllabus ID
            var subjectId = await _dbContext.Syllabus
                .Where(s => s.SyllabusId == syllabusId)
                .Select(s => s.SubjectId)
                .FirstOrDefaultAsync();

            if (subjectId == Guid.Empty)
                throw new InvalidOperationException("No subject found for the given syllabus ID.");

            // Step 2: Get all syllabuses that belong to the same subject
            var siblingSyllabuses = await _dbContext.Syllabus
                .Where(s => s.SubjectId == subjectId)
                .OrderByDescending(s => s.UpdatedAt ?? s.CreatedAt)
                .ToListAsync();

            int totalCount = siblingSyllabuses.Count;

            var paginatedItems = siblingSyllabuses
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var data = _mapper.Map<List<SyllabusListViewDto>>(paginatedItems);

            return new PagedResult<SyllabusListViewDto>
            {
                Items = data,
                TotalCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<Syllabus> ImportSyllabusAsync(SyllabusInsertDto syllabus)
        {
            if (syllabus == null)
                throw new ArgumentNullException(nameof(syllabus));

            var existingSyllabus = await _dbContext.Syllabus
                .SingleOrDefaultAsync(s => s.CourseCode == syllabus.CourseCode &&
                                           s.Status != null && s.Status.ToLower() == "active");

            if (existingSyllabus != null)
            {
                await DeleteSyllabusAsync(existingSyllabus.SyllabusId);
                //can delete syllabus child here
            }

            var newSyllabus = _mapper.Map<Syllabus>(syllabus);
            newSyllabus.SyllabusId = Guid.NewGuid();
            newSyllabus.Status = "Active";
            newSyllabus.CreatedAt = DateTime.UtcNow;
            newSyllabus.UpdatedAt = DateTime.UtcNow;

            await _dbContext.Syllabus.AddAsync(newSyllabus);
            return newSyllabus;
        }

        public async Task<bool> UpdateSyllabusAsync(Syllabus existingSyllabus, SyllabusInsertDto syllabusDto)
        {
            if (existingSyllabus == null)
                throw new ArgumentNullException(nameof(existingSyllabus), "Existing syllabus cannot be null.");

            if (syllabusDto == null)
                throw new ArgumentNullException(nameof(syllabusDto), "Syllabus data cannot be null.");

            // Use AutoMapper to update existing entity
            _mapper.Map(syllabusDto, existingSyllabus);
            existingSyllabus.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Syllabus?> GetActiveSyllabusBySubjectIdAsync(Guid subjectId)
        {
            var syllabus = await _dbContext.Syllabus
               .Where(s => s.SubjectId == subjectId && s.Status == "Active")
               .FirstOrDefaultAsync();

            return syllabus;
        }

        public async Task<SyllabusDetailDto> GetSyllabusDetailAsync(Guid? syllabusId)
        {
            if (syllabusId == null)
                throw new ArgumentNullException(nameof(syllabusId), "Syllabus ID cannot be null.");
            var syllabusDto = await _dbContext.Syllabus
                .AsNoTracking()
                .Where(s => s.SyllabusId == syllabusId)
                .ProjectTo<SyllabusDetailDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

            if (syllabusDto == null)
                throw new KeyNotFoundException($"No syllabus found with ID: {syllabusId}");
            return syllabusDto;
        }

        public async Task<Syllabus> GetSyllabusByIdAsync(Guid? syllabusId)
        {
            if (syllabusId == null)
                throw new ArgumentNullException(nameof(syllabusId), "Syllabus ID cannot be null.");
            var syllabus = await _dbContext.Syllabus
                .AsNoTracking()
                .Where(s => s.SyllabusId == syllabusId)
                .SingleOrDefaultAsync();

            if (syllabus == null)
                throw new KeyNotFoundException($"No syllabus found with ID: {syllabusId}");
            return syllabus;
        }
    }
}
