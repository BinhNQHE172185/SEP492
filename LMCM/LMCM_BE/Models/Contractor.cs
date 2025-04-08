using LMCM_BE.Models.Constant;
using System;
using System.Collections.Generic;

namespace LMCM_BE.Models;

public partial class Contractor
{
    public Guid ContractorId { get; set; }

    public string ContractorName { get; set; } = null!;

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? TaxCode { get; set; }

    public string? Email { get; set; }

    public string? EmployeeCode { get; set; }

    public string? IdCardNumber { get; set; }

    public string? IdIssuedPlace { get; set; }

    public string? Position { get; set; }

    public string? BankAccountNumber { get; set; }

    public string? BankName { get; set; }

    public GenericStatus Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
