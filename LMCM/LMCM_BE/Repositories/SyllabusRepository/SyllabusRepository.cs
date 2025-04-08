using LMCM_BE.DbContext;
using LMCM_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.SyllabusRepository
{
    public class SyllabusRepository : ISyllabusRepository
    {

        private readonly LMCM_DBContext _dbContext;
        public SyllabusRepository(LMCM_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> UpdateSyllabusAsync(Syllabus syllabus)
        {
            _dbContext.Syllabus.Update(syllabus);
            
            return true;
        }


        public async Task<(List<Syllabus>, int totalCount)> GetSyllabusesAsync(string? searchKey, int pageIndex = 1, int pageSize = 10)
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

            query = query.Where(s => s.Status == "Active");

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Include(s => s.Subject)
                .ToListAsync();

            return (items, totalCount);
        }
        public async Task<List<Syllabus>> GetSyllabusesAsync(string? searchKey)
        {
            var query = _dbContext.Syllabus.AsQueryable();

            query = query.OrderBy(s => s.CourseCode);

            query = query.Where(s => s.Status != "Inactive");

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                string search = searchKey.Trim().ToLower();
                query = query.Where(s => s.CourseCode.ToLower().Contains(search) ||
                                         s.CourseNameEnglish.ToLower().Contains(search) ||
                                         s.CourseName.ToLower().Contains(search));
            }

            var items = await query.ToListAsync();

            return items;
        }
        public async Task<(List<Syllabus>, int totalCount)> GetSyllabusChangeHistoriesAsync(
            string courseCode, string? searchKey, int pageIndex = 1, int pageSize = 10)
        {
            var siblingSyllabuses = await _dbContext.Syllabus
                .Where(s => s.CourseCode == courseCode)
                .OrderByDescending(s => s.UpdatedAt ?? s.CreatedAt)
                .ToListAsync();

            var paginatedItems = siblingSyllabuses
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            int totalCount = siblingSyllabuses.Count;

            return (paginatedItems, totalCount);
        }

        public async Task<bool> ImportSyllabusAsync(Syllabus syllabus)
        {
            // Add the new syllabus to the database
            await _dbContext.Syllabus.AddAsync(syllabus);

            return true;
        }

        public async Task<Syllabus?> GetActiveSyllabusBySubjectIdAsync(Guid subjectId)
        {
            var syllabus = await _dbContext.Syllabus
               .Where(s => s.SubjectId == subjectId && s.Status == "Active")
               .FirstOrDefaultAsync();

            return syllabus;
        }

        public async Task<Syllabus> GetSyllabusDetailAsync(Guid? syllabusId)
        {
            var syllabus = await _dbContext.Syllabus
                .AsNoTracking()
                .Include(s => s.Clos)
                .Include(s => s.ConstructivistQuestions)
                .Include(s => s.Schedules)
                .Include(s => s.GradingStructures)
                .Where(s => s.SyllabusId == syllabusId)
                .SingleOrDefaultAsync();

            return syllabus;
        }

        public async Task<Syllabus> GetSyllabusByIdAsync(Guid? syllabusId)
        {
            var syllabus = await _dbContext.Syllabus
                .AsNoTracking()
                .Where(s => s.SyllabusId == syllabusId)
                .SingleOrDefaultAsync();

            return syllabus;
        }
        public async Task<Syllabus> GetSyllabusByCourseCodeAsync(string courseCode)
        {
            var syllabus = await _dbContext.Syllabus
                .AsNoTracking()
                .Where(s => s.CourseCode == courseCode && s.Status == "Active")
                .SingleOrDefaultAsync();

            return syllabus;
        }
    }
}
