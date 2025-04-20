using LMCM_BE.Shared.Constant;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class User : IdentityUser<Guid>
{

    public string? Name { get; set; }

    public string? Picture { get; set; }

    public UserStatus Status { get; set; }

    public virtual ICollection<AcceptanceRecord> AcceptanceRecords { get; set; } = new List<AcceptanceRecord>();

    public virtual ICollection<BudgetProposal> BudgetProposals { get; set; } = new List<BudgetProposal>();

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual ICollection<DocumentTemplate> DocumentTemplates { get; set; } = new List<DocumentTemplate>();

    public virtual ICollection<LearningMaterialChangesHistory> LearningMaterialChangesHistories { get; set; } = new List<LearningMaterialChangesHistory>();
}
