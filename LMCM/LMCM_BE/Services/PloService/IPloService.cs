using LMCM_BE.DTOs.PloDtos;

namespace LMCM_BE.Services.PloService
{
    public interface IPloService
    {
        Task<List<PloDetailDto>> GetPloDetailsByCurriculumIdAsync(Guid curriculumId);
    }
}
