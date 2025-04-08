using LMCM_BE.Models.Constant;
using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class BudgetProposal
{
    public Guid ProposalId { get; set; }

    public Guid AuthorId { get; set; }

    public string Title { get; set; } = null!;

    public DateTime? ProposalDate { get; set; }

    public string? Url { get; set; }

    public GenericStatus Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User Author { get; set; } = null!;

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
