using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class BasePrintTransaction
{
    public int Id { get; set; }

    public int? BaseIssueId { get; set; }

    public int? Quantity { get; set; }

    public int? DamageQuantity { get; set; }

    public string? Remarks { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
