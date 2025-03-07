namespace LMCM_BE.DTOs.CurriculumsSubjectDtos
{
    public class TempCurriculumsSubject
    {
        public string SubjectCode { get; set; } = null!;
        public int? TermNo { get; set; }
        public int? Credit { get; set; }
        public int? Options { get; set; }
    }
}
