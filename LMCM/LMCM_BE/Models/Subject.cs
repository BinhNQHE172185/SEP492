using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class Subject
{
    public Guid SubjectId { get; set; }

    public string SubjectCode { get; set; } = null!;

    public string SubjectName { get; set; } = null!;

    public string? SubjectNameEnglish { get; set; }

    public bool? IsConstructivist { get; set; }

    public string? Method { get; set; }

    public int? Duration { get; set; }

    public int? Reality { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Plo> Plos { get; set; } = new List<Plo>();
}
