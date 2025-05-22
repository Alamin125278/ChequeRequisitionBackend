using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class Vendor
{
    public int Id { get; set; }

    public string? VendorName { get; set; }

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? PhotoPath { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsDelete { get; set; }

    public virtual ICollection<Bank> Banks { get; set; } = new List<Bank>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
