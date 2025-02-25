using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class Contract
    {
        public Contract()
        {
            AcceptanceRecords = new HashSet<AcceptanceRecord>();
        }

        public Guid ContractId { get; set; }
        public DateTime? ContractDate { get; set; }
        public Guid ContractorId { get; set; }
        public decimal ContractValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Url { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual User Contractor { get; set; } = null!;
        public virtual ICollection<AcceptanceRecord> AcceptanceRecords { get; set; }
    }
}
