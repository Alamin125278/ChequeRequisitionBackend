using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class UserRole
{
    public int Id { get; set; }

    public string? RoleName { get; set; }

    public virtual ICollection<UserRoleDefaultMenuPermission> UserRoleDefaultMenuPermissions { get; set; } = new List<UserRoleDefaultMenuPermission>();
}
