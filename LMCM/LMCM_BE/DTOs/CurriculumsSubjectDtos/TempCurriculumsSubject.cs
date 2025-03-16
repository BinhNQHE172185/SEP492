namespace LMCM_BE.DTOs.CurriculumsSubjectDtos
{
    public class TempCurriculumsSubject
    {
        public Guid SubjectId { get; set; }
        public string SubjectCode { get; set; } = null!;
        public string? SubjectName { get; set; }
        public int? TermNo { get; set; }
        public int? Credit { get; set; }
        public int? Options { get; set; }
    }
}
