using LMCM_BE.Models;

namespace LMCM_BE.Repositories.ConstructivistQuestionRepository
{
    public interface IConstructivistQuestionRepository
    {
        Task<bool> ImportConstructivistQuestionsAsync(List<ConstructivistQuestion> questions, Guid syllabusId);
        Task<bool> DeleteConstructivistQuestionsBySyllabusAsync(Guid syllabusId);
    }
}
