namespace LMCM_BE.DTOs.CurriculumDtos
{
    public class CurriculumDto
    {
        public Guid CurriculumId { get; set; }
        public string CurriculumCode { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? DecisionNo { get; set; }

        public int? TotalCredit { get; set; }

        public DateTime? ApprovedDate { get; set; }
    }
}
