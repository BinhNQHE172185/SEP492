using LMCM_BE.Models.Constant;
using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class Contract
{
    public Guid ContractId { get; set; }

    public Guid ProposalId { get; set; }

    public Guid AuthorId { get; set; }

    public Guid ContractorId { get; set; }

    public string Title { get; set; } = null!;

    public DateTime? ContractDate { get; set; }

    public decimal ContractValue { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Url { get; set; }

    public GenericStatus Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AcceptanceRecord> AcceptanceRecords { get; set; } = new List<AcceptanceRecord>();

    public virtual User Author { get; set; } = null!;

    public virtual Contractor Contractor { get; set; } = null!;

    public virtual ICollection<LearningMaterialChangesHistory> LearningMaterialChangesHistories { get; set; } = new List<LearningMaterialChangesHistory>();

    public virtual BudgetProposal Proposal { get; set; } = null!;
}
