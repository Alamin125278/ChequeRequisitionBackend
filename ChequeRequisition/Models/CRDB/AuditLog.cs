using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class AuditLog
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? ModuleName { get; set; }

    public string? ActionType { get; set; }

    public int? TargetId { get; set; }

    public string? RequestIp { get; set; }

    public DateTime? Timestamp { get; set; }

    public string? Details { get; set; }

    public virtual User User { get; set; } = null!;
}
