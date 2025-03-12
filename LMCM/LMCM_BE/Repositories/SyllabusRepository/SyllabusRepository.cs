using AutoMapper;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.DTOs.SyllabusDtos;
using LMCM_BE.Models;
using Microsoft.Data.SqlClient;
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
        public async Task<PagedResult<SyllabusChangesHistoryListDto>> GetSyllabusChangeHistoriesAsync(
            Guid? syllabusId, string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            if(syllabusId == null)
                throw new ArgumentNullException(nameof(syllabusId));

            var syllabusIdParam = new SqlParameter("@syllabusId", syllabusId);

            var previousSyllabuses = await _dbContext.Syllabus
                .FromSqlRaw(@"
                            WITH SyllabusHistory AS (
                                SELECT * FROM Syllabus WHERE Syllabus_ID = @syllabusId
                                UNION ALL
                                SELECT s.* FROM Syllabus s
                                INNER JOIN SyllabusHistory sh ON s.Syllabus_ID = sh.Previous_Version_ID
                            )
                            SELECT * FROM SyllabusHistory;", syllabusIdParam)
                .ToListAsync(); // Ensure async execution

            int totalCount = previousSyllabuses.Count;

            var paginatedItems = previousSyllabuses
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var data = _mapper.Map<List<SyllabusChangesHistoryListDto>>(paginatedItems);

            return new PagedResult<SyllabusChangesHistoryListDto>
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

            Guid? previousVersionId = null;

            if (existingSyllabus != null)
            {
                await DeleteSyllabusAsync(existingSyllabus.SyllabusId);
                previousVersionId = existingSyllabus.SyllabusId;
            }

            var newSyllabus = new Syllabus
            {
                SyllabusId = Guid.NewGuid(),
                SubjectId = syllabus.SubjectId,
                PreviousVersionId = previousVersionId,
                ProgramName = syllabus.ProgramName,
                CourseCode = syllabus.CourseCode,
                CourseName = syllabus.CourseName,
                CourseNameEnglish = syllabus.CourseNameEnglish,
                LearningTeachingMethod = syllabus.LearningTeachingMethod,
                NoOfCredits = syllabus.NoOfCredits,
                DegreeLevel = syllabus.DegreeLevel,
                TimeAllocation = syllabus.TimeAllocation,
                PreRequisite = syllabus.PreRequisite,
                Description = syllabus.Description,
                StudentTask = syllabus.StudentTask,
                Tools = syllabus.Tools,
                Note = syllabus.Note,
                MinGpaToPass = syllabus.MinGpaToPass,
                ScoringScale = syllabus.ScoringScale,
                ApprovedDate = syllabus.ApprovedDate,
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _dbContext.Syllabus.AddAsync(newSyllabus);
            return newSyllabus;
        }

        public async Task<bool> UpdateSyllabusAsync(Syllabus existingSyllabus, SyllabusInsertDto syllabusDto)
        {
            existingSyllabus.ProgramName = syllabusDto.ProgramName;
            existingSyllabus.CourseName = syllabusDto.CourseName;
            existingSyllabus.CourseNameEnglish = syllabusDto.CourseNameEnglish;
            existingSyllabus.LearningTeachingMethod = syllabusDto.LearningTeachingMethod;
            existingSyllabus.NoOfCredits = syllabusDto.NoOfCredits;
            existingSyllabus.DegreeLevel = syllabusDto.DegreeLevel;
            existingSyllabus.TimeAllocation = syllabusDto.TimeAllocation;
            existingSyllabus.PreRequisite = syllabusDto.PreRequisite;
            existingSyllabus.Description = syllabusDto.Description;
            existingSyllabus.StudentTask = syllabusDto.StudentTask;
            existingSyllabus.Tools = syllabusDto.Tools;
            existingSyllabus.Note = syllabusDto.Note;
            existingSyllabus.MinGpaToPass = syllabusDto.MinGpaToPass;
            existingSyllabus.ScoringScale = syllabusDto.ScoringScale;
            existingSyllabus.ApprovedDate = syllabusDto.ApprovedDate;
            existingSyllabus.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> HasActiveSyllabusesBySubjectIdAsync(Guid subjectId)
        {
            return await _dbContext.Syllabus
                .AnyAsync(s => s.SubjectId == subjectId && s.Status == "Active");
        }
    }
}
