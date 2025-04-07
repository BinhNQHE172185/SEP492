using LMCM_BE.Models;

namespace LMCM_BE.Repositories.PloRepository
{
    public interface IPloRepository
    {
        Task<List<Plo>> GetPloDetailsByCurriculumIdAsync(Guid curriculumId);
        Task<List<Plo>> GetPloByCurriculumIdAsync(Guid curriculumId);
        Task<bool> UpdateRangeAsync(List<Plo> entities);
        Task<bool> HasActivePloAsync(Guid curriculumId);
    }
}
