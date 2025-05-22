using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class Menu
{
    public int Id { get; set; }

    public string? MenuName { get; set; }

    public string? Icon { get; set; }

    public int? ParentId { get; set; }

    public virtual ICollection<UserMenuPermission> UserMenuPermissions { get; set; } = new List<UserMenuPermission>();

    public virtual ICollection<UserRoleDefaultMenuPermission> UserRoleDefaultMenuPermissions { get; set; } = new List<UserRoleDefaultMenuPermission>();
}
