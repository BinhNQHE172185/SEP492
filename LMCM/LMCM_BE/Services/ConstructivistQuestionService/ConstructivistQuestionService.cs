using LMCM_BE.DTOs.ConstructivistQuestionDtos;
using LMCM_BE.Repositories.ConstructivistQuestionRepository;
using LMCM_BE.Repositories.CurriculumRepository;

namespace LMCM_BE.Services.ConstructivistQuestionService
{
    public class ConstructivistQuestionService : IConstructivistQuestionService
    {
        private readonly IConstructivistQuestionRepository _questionRepository;

        public ConstructivistQuestionService(IConstructivistQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }
        public async Task<bool> DeleteConstructivistQuestionsBySyllabusAsync(Guid syllabusId)
        {
            return await _questionRepository.DeleteConstructivistQuestionsBySyllabusAsync(syllabusId);
        }

        public async Task<bool> ImportConstructivistQuestionsAsync(List<ConstructivistQuestionInsertDto> questions, Guid syllabusId)
        {
            return await _questionRepository.ImportConstructivistQuestionsAsync(questions, syllabusId);
        }
    }
}
