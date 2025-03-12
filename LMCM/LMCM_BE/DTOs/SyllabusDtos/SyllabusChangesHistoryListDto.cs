namespace LMCM_BE.DTOs.SyllabusDtos
{
    public class SyllabusChangesHistoryListDto
    {
        public Guid SyllabusId { get; set; }
        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
