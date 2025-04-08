using LMCM_BE.DbContext;
using LMCM_BE.Models;
using LMCM_BE.Shared.Constant;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.CurriculumsSubjectRepository
{
    public class CurriculumsSubjectRepository : ICurriculumsSubjectRepository
    {
        private readonly LMCM_DBContext _dbContext;

        public CurriculumsSubjectRepository(LMCM_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<CurriculumsSubject>> GetCurriculumsSubjectByCurriculumIdAsync(Guid curriculumId)
        {
            return await _dbContext.CurriculumsSubjects
                .Where(cs => cs.CurriculumId == curriculumId && cs.Status == GenericStatus.Active)
                .ToListAsync();
        }
        public async Task<bool> UpdateRangeAsync(List<CurriculumsSubject> entities)
        {
            _dbContext.CurriculumsSubjects.UpdateRange(entities);
            return true;
        }
        public async Task<bool> HasActiveCurriculumsSubjectsAsync(Guid curriculumId)
        {
            return await _dbContext.CurriculumsSubjects
                .AnyAsync(cs => cs.CurriculumId == curriculumId && cs.Status == GenericStatus.Active);
        }
        public async Task<bool> HasActiveCurriculumSubjectsBySubjectIdAsync(Guid subjectId)
        {
            return await _dbContext.CurriculumsSubjects
                .AnyAsync(cs => cs.SubjectId == subjectId && cs.Status == GenericStatus.Active);
        }
    }
}
