using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class UserMenuPermission
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? MenuId { get; set; }

    public virtual Menu? Menu { get; set; }

    public virtual User? User { get; set; }
}
