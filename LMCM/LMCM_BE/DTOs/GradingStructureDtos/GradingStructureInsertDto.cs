using LMCM_BE.Models;

namespace LMCM_BE.DTOs.GradingStructureDtos
{
    public class GradingStructureInsertDto
    {
        public Guid StructureId { get; set; }

        public int StructureNo { get; set; }

        public string? AssessmentComponent { get; set; }

        public string? AssessmentType { get; set; }

        public decimal Weight { get; set; }

        public int? Part { get; set; }

        public decimal? MinValue { get; set; }

        public string? Duration { get; set; }

        public string? Clo { get; set; }

        public string? QuestionType { get; set; }

        public string? QuestionNo { get; set; }

        public string? Scope { get; set; }

        public string? How { get; set; }

        public string? Note { get; set; }

        public int? SessionNo { get; set; }

        public string? Reference { get; set; }

        public Guid SyllabusId { get; set; }
    }
}
