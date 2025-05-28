using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class UserMenuPermission
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int MenuId { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Menu Menu { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }

    public virtual User User { get; set; } = null!;
}
