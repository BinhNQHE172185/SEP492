using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class Permission
    {
        public Guid Id { get; set; }
        public string? Type { get; set; }
        public Guid ItemId { get; set; }
    }
}
