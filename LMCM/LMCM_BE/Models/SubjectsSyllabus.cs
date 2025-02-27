using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class SubjectsSyllabus
{
    public Guid SubjectId { get; set; }

    public Guid SyllabusId { get; set; }

    public string? Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Subject Subject { get; set; } = null!;

    public virtual Syllabus Syllabus { get; set; } = null!;
}
