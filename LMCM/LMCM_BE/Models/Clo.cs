using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class Clo
{
    public Guid CloId { get; set; }

    public string? CloName { get; set; }

    public string? CloDescription { get; set; }

    public Guid SyllabusId { get; set; }

    public virtual Syllabus Syllabus { get; set; } = null!;
}
