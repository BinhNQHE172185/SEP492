using AutoMapper;
using LMCM_BE.DbContext;
using LMCM_BE.DTOs.ConstructivistQuestionDtos;
using LMCM_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace LMCM_BE.Repositories.ConstructivistQuestionRepository
{
    public class ConstructivistQuestionRepository : IConstructivistQuestionRepository
    {
        private readonly LMCM_DBContext _dbContext;
        private readonly IMapper _mapper;
        public ConstructivistQuestionRepository(LMCM_DBContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<bool> DeleteConstructivistQuestionsBySyllabusAsync(Guid syllabusId)
        {
            if (syllabusId == Guid.Empty)
                throw new ArgumentException("Syllabus ID cannot be empty.", nameof(syllabusId));

            try
            {
                var questions = await _dbContext.ConstructivistQuestions
                    .Where(s => s.SyllabusId == syllabusId)
                    .ToListAsync();

                if (!questions.Any())
                    return false; // No grading structures found for the given syllabus

                foreach (var question in questions)
                {
                    question.Status = "Inactive";
                    question.UpdatedAt = DateTime.UtcNow;
                }

                _dbContext.ConstructivistQuestions.UpdateRange(questions);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine(dbEx.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> ImportConstructivistQuestionsAsync(List<ConstructivistQuestionInsertDto> questions)
        {
            if (questions == null || !questions.Any())
                throw new ArgumentNullException(nameof(questions));

            var newQuestions = _mapper.Map<List<ConstructivistQuestion>>(questions);

            foreach (var question in newQuestions)
            {
                question.QuestionId = Guid.NewGuid();
                question.Status = "Active";
                question.CreatedAt = DateTime.UtcNow;
                question.UpdatedAt = DateTime.UtcNow;
            }

            await _dbContext.ConstructivistQuestions.AddRangeAsync(newQuestions);
            return true;
        }
    }
}
