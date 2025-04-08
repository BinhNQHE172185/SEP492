using LMCM_BE.Shared.Constant;
using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class AcceptanceRecord
{
    public Guid AcceptanceId { get; set; }

    public Guid AuthorId { get; set; }

    public Guid ContractId { get; set; }

    public string Title { get; set; } = null!;

    public DateTime? AcceptanceDate { get; set; }

    public decimal? FinalPrice { get; set; }

    public string? Url { get; set; }

    public GenericStatus Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User Author { get; set; } = null!;

    public virtual Contract Contract { get; set; } = null!;
}
