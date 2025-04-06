using LMCM_BE.DTOs.GradingStructureDtos;

namespace LMCM_BE.Repositories.GradingStructureRepository
{
    public interface IGradingStructureRepository
    {
        Task<bool> ImportGradingStructuresAsync(List<GradingStructureInsertDto> gradingStructures, Guid syllabusId);
        Task<bool> DeleteGradingStructuresBySyllabusAsync(Guid syllabusId);
    }
}
