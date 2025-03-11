using LMCM_BE.DTOs.LearningMaterialDtos;

namespace LMCM_BE.Repositories.LearningMaterialRepository
{
    public interface ILearningMaterialRepository
    {
        Task<bool> ImportLearningMaterialsAsync(List<LearningMaterialInsertDto> materials);
        Task<bool> DeleteLearningMaterialsBySyllabusAsync(Guid syllabusId);
    }
}
