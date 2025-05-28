using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class User
{
    public int Id { get; set; }

    public int? BranchId { get; set; }

    public int? BankId { get; set; }

    public int? VendorId { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? UserName { get; set; }

    public string? PasswordHash { get; set; }

    public string? ImagePath { get; set; }

    public string? Role { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual Bank? Bank { get; set; }

    public virtual ICollection<Bank> BankCreatedByNavigations { get; set; } = new List<Bank>();

    public virtual ICollection<Bank> BankUpdatedByNavigations { get; set; } = new List<Bank>();

    public virtual Branch? Branch { get; set; }

    public virtual ICollection<Branch> BranchCreatedByNavigations { get; set; } = new List<Branch>();

    public virtual ICollection<Branch> BranchUpdatedByNavigations { get; set; } = new List<Branch>();

    public virtual ICollection<Challan> ChallanCreatedByNavigations { get; set; } = new List<Challan>();

    public virtual ICollection<Challan> ChallanUpdatedByNavigations { get; set; } = new List<Challan>();

    public virtual ICollection<ChequeBookRequisition> ChequeBookRequisitionCreatedByNavigations { get; set; } = new List<ChequeBookRequisition>();

    public virtual ICollection<ChequeBookRequisition> ChequeBookRequisitionRequestedByNavigations { get; set; } = new List<ChequeBookRequisition>();

    public virtual ICollection<ChequeBookRequisition> ChequeBookRequisitionUpdatedByNavigations { get; set; } = new List<ChequeBookRequisition>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<User> InverseCreatedByNavigation { get; set; } = new List<User>();

    public virtual ICollection<User> InverseUpdatedByNavigation { get; set; } = new List<User>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<SetSerialNumber> SetSerialNumberCreatedByNavigations { get; set; } = new List<SetSerialNumber>();

    public virtual ICollection<SetSerialNumber> SetSerialNumberUpdatedByNavigations { get; set; } = new List<SetSerialNumber>();

    public virtual ICollection<StatusHistory> StatusHistories { get; set; } = new List<StatusHistory>();

    public virtual User? UpdatedByNavigation { get; set; }

    public virtual ICollection<UserMenuPermission> UserMenuPermissionCreatedByNavigations { get; set; } = new List<UserMenuPermission>();

    public virtual ICollection<UserMenuPermission> UserMenuPermissionUpdatedByNavigations { get; set; } = new List<UserMenuPermission>();

    public virtual ICollection<UserMenuPermission> UserMenuPermissionUsers { get; set; } = new List<UserMenuPermission>();

    public virtual Vendor? Vendor { get; set; }
}
