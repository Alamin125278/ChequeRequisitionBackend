using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class ProductionPrint
{
    public int Id { get; set; }

    public int? ProductionIssueId { get; set; }

    public int? PrintedQty { get; set; }

    public DateOnly? PrintDate { get; set; }

    public string? Remarks { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
