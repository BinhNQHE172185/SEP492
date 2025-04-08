using LMCM_BE.DbContext;
using LMCM_BE.Models;
using LMCM_BE.Models.Constant;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.PloSubjectRepository
{
    public class PloSubjectRepository : IPloSubjectRepository
    {
        private readonly LMCM_DBContext _dbContext;

        public PloSubjectRepository(LMCM_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<PloSubject>> GetPloSubjectByCurriculumIdAsync(Guid curriculumId)
        {
            return await _dbContext.Plos
                .Where(p => p.CurriculumId == curriculumId)
                .SelectMany(p => p.PloSubjects)
                .Where(ps => ps.Status == GenericStatus.Active)
                .ToListAsync();
        }

        public async Task<bool> UpdateRangeAsync(List<PloSubject> entities)
        {
            _dbContext.PloSubjects.UpdateRange(entities);
            return true;
        }
        public async Task<bool> HasActivePloSubjectByCurriculumIdAsync(Guid curriculumId)
        {
            return await _dbContext.PloSubjects
                .AnyAsync(ps => ps.Plo.CurriculumId == curriculumId && ps.Status == GenericStatus.Active);
        }
        public async Task<bool> HasActivePloSubjectBySubjectIdAsync(Guid subjectId)
        {
            return await _dbContext.PloSubjects
                .AnyAsync(ps => ps.SubjectId == subjectId && ps.Status == GenericStatus.Active);
        }


    }
}
