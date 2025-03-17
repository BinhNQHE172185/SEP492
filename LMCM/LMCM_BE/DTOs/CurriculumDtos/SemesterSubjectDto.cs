namespace LMCM_BE.DTOs.CurriculumDtos
{
    public class SemesterSubjectDto
    {
            public Guid SubjectId { get; set; }
            public string SubjectCode { get; set; } = null!;
            public string? SubjectName { get; set; }
            public string? SubjectNameEnglish { get; set; }
            public int? Credit { get; set; }
            public int? Options { get; set; }
            public int? Duration { get; set; }
            public int? Reality { get; set; }
    }
}
