using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class UserRole
{
    public int Id { get; set; }

    public string? RoleName { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<UserRoleDefaultMenuPermission> UserRoleDefaultMenuPermissions { get; set; } = new List<UserRoleDefaultMenuPermission>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
