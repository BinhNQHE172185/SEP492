using LMCM_BE.DTOs.LearningMaterialDtos;

namespace LMCM_BE.Services.LearningMaterialService
{
    public interface ILearningMaterialService
    {
        Task<bool> ImportLearningMaterialsAsync(List<LearningMaterialInsertDto> materials);
        Task<bool> DeleteLearningMaterialsBySyllabusAsync(Guid syllabusId);
    }
}
