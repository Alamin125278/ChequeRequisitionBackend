using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class UserRoleDefaultMenuPermission
{
    public int Id { get; set; }

    public int? RoleId { get; set; }

    public int? MenuId { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Menu? Menu { get; set; }

    public virtual UserRole? Role { get; set; }
}
