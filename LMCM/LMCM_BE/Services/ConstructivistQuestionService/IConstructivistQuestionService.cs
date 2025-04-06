using LMCM_BE.DTOs.ConstructivistQuestionDtos;

namespace LMCM_BE.Services.ConstructivistQuestionService
{
    public interface IConstructivistQuestionService
    {
        Task<bool> ImportConstructivistQuestionsAsync(List<ConstructivistQuestionInsertDto> questions, Guid syllabusId);
        Task<bool> DeleteConstructivistQuestionsBySyllabusAsync(Guid syllabusId);
    }
}
