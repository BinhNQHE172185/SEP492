namespace LMCM_BE.DTOs.ConstructivistQuestionDtos
{
    public class ConstructivistQuestionInsertDto
    {
        public Guid QuestionId { get; set; }

        public int SessionNo { get; set; }

        public string? QuestionName { get; set; }

        public string? QuestionDetail { get; set; }

        public Guid SyllabusId { get; set; }
    }
}
