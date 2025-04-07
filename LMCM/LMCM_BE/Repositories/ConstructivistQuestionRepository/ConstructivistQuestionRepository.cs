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
        public ConstructivistQuestionRepository(LMCM_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> DeleteConstructivistQuestionsBySyllabusAsync(Guid syllabusId)
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

            return true;
        }

        public async Task<bool> ImportConstructivistQuestionsAsync(List<ConstructivistQuestion> questions, Guid syllabusId)
        {
            if (questions == null || !questions.Any())
                return false;

            foreach (var question in questions)
            {
                question.SyllabusId = syllabusId;
                question.QuestionId = Guid.NewGuid();
                question.Status = "Active";
                question.CreatedAt = DateTime.UtcNow;
                question.UpdatedAt = DateTime.UtcNow;
            }

            await _dbContext.ConstructivistQuestions.AddRangeAsync(questions);

            return true;
        }
    }
}
