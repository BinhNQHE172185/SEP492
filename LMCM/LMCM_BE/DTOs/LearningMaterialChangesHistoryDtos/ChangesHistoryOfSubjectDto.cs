using LMCM_BE.DTOs.ContractDtos;
using LMCM_BE.DTOs.SyllabusDtos;
using LMCM_BE.DTOs.UserDtos;
using LMCM_BE.Models;
using LMCM_BE.Shared.Constant;

namespace LMCM_BE.DTOs.LearningMaterialDtos
{
    public class ChangesHistoryOfSubjectDto
    {
        public Guid HistoryId { get; set; }
        public Guid UserId { get; set; }
        public Guid? ContractId { get; set; }
        public Guid SyllabusId { get; set; }
        public string ChangeType { get; set; } = null!;
        public string? ChangeDescription { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string? StartTerm { get; set; }
        public string? CourseCode { get; set; }
        public GenericStatus Status { get; set; }
        public virtual ContractDetailDto? Contract { get; set; }
        public virtual SyllabusDetailDto Syllabus { get; set; } = null!;
        public ListUserResponseDto User { get; set; } = null!;
    }
}
