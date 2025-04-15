namespace LMCM_BE.DTOs.SubjectDtos
{
    public class SubjectDto
    {
        public Guid SubjectId { get; set; }
        public string SubjectCode { get; set; } = null!;

        public string SubjectName { get; set; } = null!;

        public string? SubjectNameEnglish { get; set; }

        public bool? IsConstructivist { get; set; }

        public string? Method { get; set; }

        public int? Duration { get; set; }

        public int? Reality { get; set; }
    }
}
