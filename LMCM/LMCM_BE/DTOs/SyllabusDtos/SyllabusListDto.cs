using LMCM_BE.DTOs.SubjectDtos;
using LMCM_BE.Models;

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
        public string SubjectName { get; set; } = null!;
        public string SubjectCode { get; set; } = null!;
    }
}
