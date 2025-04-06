using LMCM_BE.DTOs.GradingStructureDtos;
using LMCM_BE.Repositories.GradingStructureRepository;

namespace LMCM_BE.Services.GradingStructureService
{
    public class GradingStructureService : IGradingStructureService
    {
        private readonly IGradingStructureRepository _gradingStructureRepository;
        public GradingStructureService(IGradingStructureRepository gradingStructureRepository)
        {
            _gradingStructureRepository = gradingStructureRepository;
        }
        public async Task<bool> DeleteGradingStructuresBySyllabusAsync(Guid syllabusId)
        {
            return await _gradingStructureRepository.DeleteGradingStructuresBySyllabusAsync(syllabusId);
        }

        public async Task<bool> ImportGradingStructuresAsync(List<GradingStructureInsertDto> gradingStructures, Guid syllabusId)
        {
            return await _gradingStructureRepository.ImportGradingStructuresAsync(gradingStructures,syllabusId);
        }
    }
}
