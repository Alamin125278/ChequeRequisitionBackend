using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class Bank
{
    public int Id { get; set; }

    public int? VendorId { get; set; }

    public string? BankName { get; set; }

    public string? BankCode { get; set; }

    public string? RoutingNumber { get; set; }

    public string? BankEmail { get; set; }

    public int? BankPhone { get; set; }

    public string? BankAddress { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Branch> Branches { get; set; } = new List<Branch>();

    public virtual ICollection<ChequeBookRequisition> ChequeBookRequisitions { get; set; } = new List<ChequeBookRequisition>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? UpdatedByNavigation { get; set; }

    public virtual Vendor? Vendor { get; set; }
}
