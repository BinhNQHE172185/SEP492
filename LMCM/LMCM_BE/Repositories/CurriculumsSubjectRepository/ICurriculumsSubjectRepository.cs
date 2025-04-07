using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Repositories.CurriculumsSubjectRepository
{
    public interface ICurriculumsSubjectRepository
    {
        Task<List<CurriculumsSubject>> GetCurriculumsSubjectByCurriculumIdAsync(Guid curriculumId);
        Task<bool> UpdateRangeAsync(List<CurriculumsSubject> entities);
        Task<bool> HasActiveCurriculumsSubjectsAsync(Guid curriculumId);
        Task<bool> HasActiveCurriculumSubjectsBySubjectIdAsync(Guid subjectId);
    }
}
