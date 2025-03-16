namespace LMCM_BE.DTOs.PloSubjectDtos
{
    public class TempPloSubject
    {
        public Guid SubjectId { get; set; }
        public string SubjectCode { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
    }
}
