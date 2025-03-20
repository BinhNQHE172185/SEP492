using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class LearningMaterialChangesHistory
{
    public Guid HistoryId { get; set; }

    public Guid UserId { get; set; }

    public Guid? ContractId { get; set; }

    public Guid NewMaterialId { get; set; }

    public Guid? OldMaterialId { get; set; }

    public string LearningMaterialType { get; set; } = null!;

    public string ChangeType { get; set; } = null!;

    public string? ChangeDescription { get; set; }

    public DateTime? CompletionDate { get; set; }

    public string? StartTerm { get; set; }

    public string? CourseCode { get; set; }

    public string? Status { get; set; }

    public virtual Contract? Contract { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual LearningMaterial NewMaterial { get; set; } = null!;
    public virtual LearningMaterial? OldMaterial { get; set; }
}
