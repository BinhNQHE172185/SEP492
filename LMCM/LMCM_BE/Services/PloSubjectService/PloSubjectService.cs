using LMCM_BE.Repositories.PloSubjectRepository;
using LMCM_BE.Shared.Constant;

namespace LMCM_BE.Services.PloSubjectService
{
    public class PloSubjectService : IPloSubjectService
    {
        private readonly IPloSubjectRepository _ploSubjectRepository;

        public PloSubjectService(IPloSubjectRepository ploSubjectRepository)
        {
            _ploSubjectRepository = ploSubjectRepository;
        }

        public async Task<bool> DeletePloSubjectsAsync(Guid curriculumId)
        {
            // Get all active PLO IDs that belong to this Curriculum
            var activePloSubject = await _ploSubjectRepository.GetPloSubjectByCurriculumIdAsync(curriculumId);

            if (!activePloSubject.Any()) return false;


            foreach (var ps in activePloSubject)
            {
                ps.Status = GenericStatus.Inactive;
                ps.UpdatedAt = DateTime.UtcNow;
            }

            // Use the repository to soft-delete all related PLO-Subjects
            return await _ploSubjectRepository.UpdateRangeAsync(activePloSubject);
        }
    }
}