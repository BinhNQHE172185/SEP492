using LMCM_BE.Models.Constant;
using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class Plo
{
    public Guid PloId { get; set; }

    public Guid CurriculumId { get; set; }

    public string PloName { get; set; } = null!;

    public string? PloDescription { get; set; }

    public GenericStatus Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Curriculum Curriculum { get; set; } = null!;

    public virtual ICollection<PloSubject> PloSubjects { get; set; } = new List<PloSubject>();
}
