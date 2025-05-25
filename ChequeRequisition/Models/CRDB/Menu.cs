using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class Menu
{
    public int Id { get; set; }

    public string? MenuName { get; set; }

    public string? Title { get; set; }

    public string? Path { get; set; }

    public string? Icon { get; set; }

    public int? ParentId { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<UserMenuPermission> UserMenuPermissions { get; set; } = new List<UserMenuPermission>();

    public virtual ICollection<UserRoleDefaultMenuPermission> UserRoleDefaultMenuPermissions { get; set; } = new List<UserRoleDefaultMenuPermission>();
}
