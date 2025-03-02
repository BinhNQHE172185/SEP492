using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace LMCM_BE.Models;

public partial class User : IdentityUser<Guid>
{

    public string? Name { get; set; }

    public string? Picture { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<AcceptanceRecord> AcceptanceRecords { get; set; } = new List<AcceptanceRecord>();

    public virtual ICollection<BudgetProposal> BudgetProposals { get; set; } = new List<BudgetProposal>();

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual ICollection<DocumentTemplate> DocumentTemplates { get; set; } = new List<DocumentTemplate>();

    public virtual ICollection<HistoryOfChange> HistoryOfChanges { get; set; } = new List<HistoryOfChange>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
