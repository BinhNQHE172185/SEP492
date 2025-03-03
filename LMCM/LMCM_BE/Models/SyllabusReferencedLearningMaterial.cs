using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class SyllabusReferencedLearningMaterial
{
    public Guid SyllabusId { get; set; }

    public Guid MaterialId { get; set; }

    public string? Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ReferencedLearningMaterial Material { get; set; } = null!;

    public virtual Syllabus Syllabus { get; set; } = null!;
}
