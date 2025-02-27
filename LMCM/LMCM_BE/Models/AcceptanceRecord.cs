using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class AcceptanceRecord
{
    public Guid AcceptanceId { get; set; }

    public Guid ContractId { get; set; }

    public DateTime? AcceptanceDate { get; set; }

    public decimal? FinalPrice { get; set; }

    public string? Url { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Contract Contract { get; set; } = null!;
}
