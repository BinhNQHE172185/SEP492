using LMCM_BE.DTOs.ConstructivistQuestionDtos;

namespace LMCM_BE.Repositories.ConstructivistQuestionRepository
{
    public interface IConstructivistQuestionRepository
    {
        Task<bool> ImportConstructivistQuestionsAsync(List<ConstructivistQuestionInsertDto> questions, Guid syllabusId);
        Task<bool> DeleteConstructivistQuestionsBySyllabusAsync(Guid syllabusId);
    }
}
