namespace LMCM_BE.DTOs.SubjectDtos
{
    public class SubjectInsertDto
    {
        public Guid SubjectId { get; set; }
        public string SubjectCode { get; set; } = null!;
        public string? PreviousSubjectCode { get; set; }
        public string SubjectName { get; set; } = null!;

        public string? PreviousCode { get; set; }

        public string? SubjectNameEnglish { get; set; }

        public bool? IsConstructivist { get; set; }

        public string Method { get; set; } = null!; 

        public int Duration { get; set; }  

        public int Reality { get; set; }  
    }
}
