using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class Branch
{
    public int Id { get; set; }

    public int? BankId { get; set; }

    public string? BranchName { get; set; }

    public string? BranchCode { get; set; }

    public string? BranchEmail { get; set; }

    public string? BranchAddress { get; set; }

    public string? BranchPhone { get; set; }

    public string? RoutingNo { get; set; }

    public bool? IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Bank? Bank { get; set; }

    public virtual ICollection<Challan> Challans { get; set; } = new List<Challan>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? UpdatedByNavigation { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
