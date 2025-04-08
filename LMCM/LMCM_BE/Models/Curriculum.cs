using LMCM_BE.Shared.Constant;
using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class Curriculum
{
    public Guid CurriculumId { get; set; }

    public string CurriculumCode { get; set; } = null!;

    public string CurriculumName { get; set; } = null!;

    public string? CurriculumNameEnglish { get; set; }

    public string? CurriculumDescription { get; set; }

    public string? VocationalCode { get; set; }

    public string? VocationalName { get; set; }

    public string? EnglishVocationalName { get; set; }

    public string? DecisionNo { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public GenericStatus Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CurriculumsSubject> CurriculumsSubjects { get; set; } = new List<CurriculumsSubject>();

    public virtual ICollection<Plo> Plos { get; set; } = new List<Plo>();
}
