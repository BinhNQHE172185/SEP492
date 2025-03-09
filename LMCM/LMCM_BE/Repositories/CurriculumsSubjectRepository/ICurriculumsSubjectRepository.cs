using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Repositories.CurriculumsSubjectRepository
{
    public interface ICurriculumsSubjectRepository
    {
        Task<PagedResult<CurriculumsSubject>> GetCurriculumsSubjectsAsync(Guid curriculumId, int pageIndex = 1, int pageSize = 10);
        Task<List<CurriculumsSubject>> GetAllCurriculumsSubjectsAsync(Guid curriculumId);
        Task<bool> AddCurriculumsSubjectsAsync(List<CurriculumsSubject> curriculumsSubjects);
        Task<bool> DeleteCurriculumsSubjectAsync(Guid curriculumId);
        Task<bool> HasActiveCurriculumsSubjectsAsync(Guid curriculumId);
        Task<bool> HasActiveCurriculumSubjectsBySubjectIdAsync(Guid subjectId);
    }
}
