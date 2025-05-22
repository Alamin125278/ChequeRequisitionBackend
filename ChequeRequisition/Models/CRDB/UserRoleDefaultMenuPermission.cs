using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class UserRoleDefaultMenuPermission
{
    public int Id { get; set; }

    public int? RoleId { get; set; }

    public int? MenuId { get; set; }

    public virtual Menu? Menu { get; set; }

    public virtual UserRole? Role { get; set; }
}
