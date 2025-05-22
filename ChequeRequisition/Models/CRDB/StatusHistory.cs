using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class StatusHistory
{
    public int Id { get; set; }

    public string? ModuleName { get; set; }

    public int? ModulePrimaryId { get; set; }

    public int? StatusId { get; set; }

    public int? ChangedBy { get; set; }

    public string? Remarks { get; set; }

    public DateTime? ChangedAt { get; set; }

    public virtual User? ChangedByNavigation { get; set; }

    public virtual Status? Status { get; set; }
}
