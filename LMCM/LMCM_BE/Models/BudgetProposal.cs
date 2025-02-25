using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class BudgetProposal
{
    public Guid ProposalId { get; set; }

    public Guid AuthorId { get; set; }

    public DateTime? ProposalDate { get; set; }

    public string? Url { get; set; }

    public string? Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual User Author { get; set; } = null!;
}
