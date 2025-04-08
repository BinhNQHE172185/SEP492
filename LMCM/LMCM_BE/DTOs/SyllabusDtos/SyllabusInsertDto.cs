namespace LMCM_BE.DTOs.SyllabusDtos
{
    public class SyllabusInsertDto
    {
        public Guid SyllabusId { get; set; }

        public Guid SubjectId { get; set; }

        public string ProgramName { get; set; } = null!;

        public string? DecisionNo { get; set; }

        public string CourseName { get; set; } = null!;

        public string? CourseNameEnglish { get; set; }

        public string CourseCode { get; set; } = null!;

        public string? LearningTeachingMethod { get; set; }

        public int? NoOfCredits { get; set; }

        public string? DegreeLevel { get; set; }

        public string? TimeAllocation { get; set; }

        public string? PreRequisite { get; set; }

        public string? Description { get; set; }

        public string? StudentTask { get; set; }

        public string? Tools { get; set; }

        public string? Note { get; set; }

        public decimal? MinGpaToPass { get; set; }

        public decimal? ScoringScale { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
