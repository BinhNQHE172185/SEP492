using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class SubjectsSyllabus
{
    public Guid SubjectId { get; set; }

    public Guid SyllabusId { get; set; }

    public virtual Subject Subject { get; set; } = null!;

    public virtual Syllabus Syllabus { get; set; } = null!;
}
