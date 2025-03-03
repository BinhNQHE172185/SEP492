using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class Schedule
{
    public Guid ScheduleId { get; set; }

    public int ScheduleNo { get; set; }

    public string? Method { get; set; }

    public string? Clo { get; set; }

    public string? Itu { get; set; }

    public string? StudentMaterial { get; set; }

    public string? StudentTask { get; set; }

    public string? StudentMaterialUrl { get; set; }

    public string? LecturerMaterial { get; set; }

    public string? LecturerTask { get; set; }

    public string? LecturerMaterialUrl { get; set; }

    public Guid SyllabusId { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Syllabus Syllabus { get; set; } = null!;
}
