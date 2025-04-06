using LMCM_BE.DTOs.GradingStructureDtos;

namespace LMCM_BE.Services.GradingStructureService
{
    public interface IGradingStructureService
    {
        Task<bool> ImportGradingStructuresAsync(List<GradingStructureInsertDto> gradingStructures, Guid syllabusId);
        Task<bool> DeleteGradingStructuresBySyllabusAsync(Guid syllabusId);
    }
}
