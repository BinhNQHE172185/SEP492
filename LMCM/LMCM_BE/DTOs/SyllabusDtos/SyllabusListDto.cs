
namespace LMCM_BE.DTOs.SyllabusDtos
{
    public class SyllabusListViewDto
    {
        public Guid SyllabusId { get; set; }
        public Guid SubjectId { get; set; }
        public string CourseName { get; set; } = null!;
        public string? CourseNameEnglish { get; set; }
        public string CourseCode { get; set; } = null!;
        public string? DecisionNo { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsActive { get; set; }
    }
}
