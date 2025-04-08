using AutoMapper;
using LMCM_BE.DTOs.PloDtos;
using LMCM_BE.Repositories.PloRepository;
using LMCM_BE.Shared.Constant;

namespace LMCM_BE.Services.PloService
{
    public class PloService: IPloService
    {
        private readonly IPloRepository _repository;
        private readonly IMapper _mapper;

        public PloService(IPloRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<List<PloDetailDto>> GetPloDetailsByCurriculumIdAsync(Guid curriculumId)
        {
            var plos = await _repository.GetPloDetailsByCurriculumIdAsync(curriculumId);
            return _mapper.Map<List<PloDetailDto>>(plos);
        }
        public async Task<bool> DeletePlosAsync(Guid curriculumId)
        {
            var plos = await _repository.GetPloByCurriculumIdAsync(curriculumId);
            if (plos.Count == 0) return false;

            foreach (var plo in plos)
            {
                plo.Status = GenericStatus.Inactive;
                plo.UpdatedAt = DateTime.UtcNow;
            }

            return await _repository.UpdateRangeAsync(plos);
        }
    }
}
