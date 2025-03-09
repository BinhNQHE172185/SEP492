using LMCM_BE.DTOs.ShareDtos;
using LMCM_BE.Models;

namespace LMCM_BE.Repositories.PloSubjectRepository
{
    public interface IPloSubjectRepository
    {
        Task<PagedResult<PloSubject>> GetPloSubjectsAsync(Guid ploId, int pageIndex = 1, int pageSize = 10);
        Task<List<PloSubject>> GetAllPloSubjectsAsync(Guid ploId);
        Task<bool> AddPloSubjectsAsync(List<PloSubject> ploSubjects);

        Task<bool> DeletePloSubjectsAsync(List<Guid> ploIds);
        Task<bool> HasActivePloSubjectByCurriculumIdAsync(Guid curriculumId);
        Task<bool> HasActivePloSubjectByPloIdAsync(Guid ploId);
        Task<bool> HasActivePloSubjectBySubjectIdAsync(Guid subjectId);
    }
}
