using LMCM_BE.Shared.Constant;
using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class DocumentTemplate
{
    public Guid TemplateId { get; set; }

    public string? TemplateType { get; set; }

    public string TemplateName { get; set; } = null!;

    public Guid AuthorId { get; set; }

    public string? Url { get; set; }

    public DocumentTemplateStatus Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User Author { get; set; } = null!;
}
