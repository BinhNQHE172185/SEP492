using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class HistoryOfChange
{
    public Guid HistoryId { get; set; }

    public string? Description { get; set; }

    public string? ItemType { get; set; }

    public Guid? ItemIdNew { get; set; }

    public Guid? ItemIdOld { get; set; }

    public DateTime? CreatedAt { get; set; }
}
