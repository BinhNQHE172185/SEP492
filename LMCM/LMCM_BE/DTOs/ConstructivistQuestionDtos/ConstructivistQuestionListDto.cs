namespace LMCM_BE.DTOs.ConstructivistQuestionDtos
{
    public class ConstructivistQuestionListDto
    {
        public Guid QuestionId { get; set; }

        public int SessionNo { get; set; }

        public string? QuestionName { get; set; }

        public string? QuestionDetail { get; set; }
    }
}
