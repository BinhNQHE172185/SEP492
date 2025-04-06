using LMCM_BE.Models;

namespace LMCM_BE.Repositories.GradingStructureRepository
{
    public interface IGradingStructureRepository
    {
        Task<bool> ImportGradingStructuresAsync(List<GradingStructure> gradingStructures, Guid syllabusId);
        Task<bool> DeleteGradingStructuresBySyllabusAsync(Guid syllabusId);
    }
}
