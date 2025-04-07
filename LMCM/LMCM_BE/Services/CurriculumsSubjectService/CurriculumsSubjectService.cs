using LMCM_BE.Repositories.CurriculumsSubjectRepository;

namespace LMCM_BE.Services.CurriculumsSubjectService
{
    public class CurriculumsSubjectService : ICurriculumsSubjectService
    {
        private readonly ICurriculumsSubjectRepository _curriculumSubjectRepository;

        public CurriculumsSubjectService(
            ICurriculumsSubjectRepository curriculumSubjectRepository
            )
        {
            _curriculumSubjectRepository = curriculumSubjectRepository;
        }
        public async Task<bool> DeleteCurriculumsSubjectAsync(Guid curriculumId)
        {
            var csList = await _curriculumSubjectRepository.GetCurriculumsSubjectByCurriculumIdAsync(curriculumId);

            if (csList.Count == 0) return false;

            foreach (var cs in csList)
            {
                cs.Status = "Inactive";
                cs.UpdatedAt = DateTime.UtcNow;
            }

            return await _curriculumSubjectRepository.UpdateRangeAsync(csList);
        }

    }
}
