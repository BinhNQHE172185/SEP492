using LMCM_BE.DTOs.PloSubjectDtos;

namespace LMCM_BE.DTOs.PloDtos
{
    public class PloDetailDto
    {
        public Guid PloId { get; set; }
        public string PloName { get; set; } = null!;
        public string? PloDescription { get; set; }
        public List<TempPloSubject> Subjects { get; set; } = new();
    }
}
