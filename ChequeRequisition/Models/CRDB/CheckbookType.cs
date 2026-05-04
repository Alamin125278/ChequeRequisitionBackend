using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class CheckbookType
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public string? Remarks { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
