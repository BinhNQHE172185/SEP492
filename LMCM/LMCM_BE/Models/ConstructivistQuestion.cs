using LMCM_BE.Shared.Constant;
using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class ConstructivistQuestion
{
    public Guid QuestionId { get; set; }

    public int SessionNo { get; set; }

    public string? QuestionName { get; set; }

    public string? QuestionDetail { get; set; }

    public Guid SyllabusId { get; set; }

    public GenericStatus Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Syllabus Syllabus { get; set; } = null!;
}
