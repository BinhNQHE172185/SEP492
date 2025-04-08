using LMCM_BE.Models;

namespace LMCM_BE.Repositories.GradingStructureRepository
{
    public interface IGradingStructureRepository
    {
        Task <List<GradingStructure>> GetGradingStructuresBySyllabusAsync(Guid syllabusId);
        Task<bool> AddGradingStructuresAsync(List<GradingStructure> gradingStructures);
        Task<bool> UpdateGradingStructuresAsync(List<GradingStructure> gradingStructures);
    }
}
