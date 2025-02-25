using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class SyllabusLearningMaterial
{
    public Guid SyllabusId { get; set; }

    public Guid MaterialId { get; set; }

    public virtual LearningMaterial Material { get; set; } = null!;

    public virtual Syllabus Syllabus { get; set; } = null!;
}
