using LMCM_BE.DTOs.CurriculumsSubjectDtos;
using LMCM_BE.Shared.Constant;

namespace LMCM_BE.DTOs.CurriculumDtos
{
    public class CurriculumDetailDto
    {
        public Guid CurriculumId { get; set; }
        public string CurriculumCode { get; set; } = null!;
        public string CurriculumName { get; set; } = null!;
        public string? CurriculumNameEnglish { get; set; }
        public string? CurriculumDescription { get; set; }
        public string? VocationalCode { get; set; }
        public string? VocationalName { get; set; }
        public string? EnglishVocationalName { get; set; }
        public string? DecisionNo { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public GenericStatus Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<SemesterDto> Semesters { get; set; } = new();
    }
}
