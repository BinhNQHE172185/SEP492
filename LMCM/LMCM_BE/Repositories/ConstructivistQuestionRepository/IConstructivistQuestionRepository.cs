using LMCM_BE.Models;

namespace LMCM_BE.Repositories.ConstructivistQuestionRepository
{
    public interface IConstructivistQuestionRepository
    {
        Task<List<ConstructivistQuestion>> GetConstructivistQuestionsBySyllabusAsync(Guid syllabusId);
        Task<bool> AddConstructivistQuestionsAsync(List<ConstructivistQuestion> questions);
        Task<bool> UpdateConstructivistQuestionsAsync(List<ConstructivistQuestion> questions);
    }
}
