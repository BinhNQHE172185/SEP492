using LMCM_BE.DTOs.CurriculumsSubjectDtos;

namespace LMCM_BE.DTOs.CurriculumDtos
{
    public class SemesterDto
    {
        public string? Name { get; set; }
        public int? Number { get; set; }
        public int? SubjectCount { get; set; }
        public List<SemesterSubjectDto>? semesterSubjects { get; set; }
    }
}
