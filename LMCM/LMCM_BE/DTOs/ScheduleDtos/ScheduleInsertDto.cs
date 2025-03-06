using LMCM_BE.Models;

namespace LMCM_BE.DTOs.ScheduleDtos
{
    public class ScheduleInsertDto
    {
        public Guid ScheduleId { get; set; }

        public int ScheduleNo { get; set; }

        public string? Method { get; set; }
        public string? Content { get; set; }
        public string? Clos { get; set; }

        public string? Itu { get; set; }

        public string? StudentMaterial { get; set; }

        public string? StudentTask { get; set; }

        public string? StudentMaterialUrl { get; set; }

        public string? LecturerMaterial { get; set; }

        public string? LecturerTask { get; set; }

        public string? LecturerMaterialUrl { get; set; }

        public Guid SyllabusId { get; set; }
    }
}
