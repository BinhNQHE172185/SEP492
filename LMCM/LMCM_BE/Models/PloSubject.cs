using LMCM_BE.Models.Constant;
using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class PloSubject
{
    public Guid PloId { get; set; }

    public Guid SubjectId { get; set; }

    public GenericStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Plo Plo { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;
}
