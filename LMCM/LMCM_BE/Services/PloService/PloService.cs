using LMCM_BE.DTOs.PloDtos;
using LMCM_BE.Repositories.PloRepository;

namespace LMCM_BE.Services.PloService
{
    public class PloService: IPloService
    {
        private readonly IPloRepository _repository;
        public PloService(IPloRepository repository)
        {
            _repository = repository;
        }
       public async Task<List<PloDetailDto>> GetPloDetailsByCurriculumIdAsync(Guid curriculumId)
        {
            return await _repository.GetPloDetailsByCurriculumIdAsync(curriculumId);
        }

    }
}
