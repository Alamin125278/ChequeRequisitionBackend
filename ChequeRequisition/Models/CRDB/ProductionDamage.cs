using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class ProductionDamage
{
    public long Id { get; set; }

    public long? ProductionIssueId { get; set; }

    public int? DamageQty { get; set; }

    public string? DamageType { get; set; }

    public DateOnly? DamageDate { get; set; }

    public string? Remarks { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
