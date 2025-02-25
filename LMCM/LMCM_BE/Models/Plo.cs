using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class Plo
{
    public Guid PloId { get; set; }

    public Guid CurriculumId { get; set; }

    public Guid SubjectId { get; set; }

    public string PloName { get; set; } = null!;

    public string? PloDescription { get; set; }

    public string? Status { get; set; }

    public virtual Curriculum Curriculum { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;
}
