using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class Permission
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? Type { get; set; }

    public Guid ItemId { get; set; }

    public virtual User User { get; set; } = null!;
}
