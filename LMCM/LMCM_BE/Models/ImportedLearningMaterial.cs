using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class ImportedLearningMaterial
{
    public Guid MaterialId { get; set; }

    public string? MaterialName { get; set; }

    public string? Type { get; set; }

    public string? MaterialQuantity { get; set; }

    public int? MaterialNo { get; set; }

    public string? Url { get; set; }

    public Guid SyllabusId { get; set; }

    public string? Purpose { get; set; }

    public string? Note { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Syllabus Syllabus { get; set; } = null!;
}
