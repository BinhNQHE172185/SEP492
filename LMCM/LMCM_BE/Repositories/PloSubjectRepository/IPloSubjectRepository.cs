using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Repositories.PloSubjectRepository
{
    public interface IPloSubjectRepository
    {
        Task<List<PloSubject>> GetPloSubjectByCurriculumIdAsync(Guid curriculumId);
        Task<bool> UpdateRangeAsync(List<PloSubject> entities);
        Task<bool> HasActivePloSubjectByCurriculumIdAsync(Guid curriculumId);
        Task<bool> HasActivePloSubjectBySubjectIdAsync(Guid subjectId);
    }
}
