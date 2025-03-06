namespace LMCM_BE.DTOs.CLODtos
{
    public class CLOInsertDto
    {
        public Guid CloId { get; set; }

        public string? CloName { get; set; }

        public string? CloDescription { get; set; }

        public Guid SyllabusId { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
