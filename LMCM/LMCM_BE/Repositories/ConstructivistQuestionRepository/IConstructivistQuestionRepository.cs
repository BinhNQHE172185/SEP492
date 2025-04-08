using LMCM_BE.Models;

namespace LMCM_BE.Repositories.ConstructivistQuestionRepository
{
    public interface IConstructivistQuestionRepository
    {
        Task<bool> AddConstructivistQuestionsAsync(List<ConstructivistQuestion> questions);
        Task<bool> DeleteConstructivistQuestionsBySyllabusAsync(Guid syllabusId);
    }
}
