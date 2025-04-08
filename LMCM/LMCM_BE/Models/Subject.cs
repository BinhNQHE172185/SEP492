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

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CurriculumsSubject> CurriculumsSubjects { get; set; } = new List<CurriculumsSubject>();

    public virtual ICollection<PloSubject> PloSubjects { get; set; } = new List<PloSubject>();

    public virtual ICollection<Syllabus> Syllabus { get; set; } = new List<Syllabus>();
}
