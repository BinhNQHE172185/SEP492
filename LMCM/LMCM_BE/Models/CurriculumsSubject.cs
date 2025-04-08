using LMCM_BE.Models.Constant;
using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class CurriculumsSubject
{
    public Guid CurriculumId { get; set; }

    public Guid SubjectId { get; set; }

    public int? TermNo { get; set; }

    public int? Credit { get; set; }

    public int? Options { get; set; }

    public GenericStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Curriculum Curriculum { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;
}
