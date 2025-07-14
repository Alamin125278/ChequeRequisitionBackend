using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class Challan
{
    public int Id { get; set; }

    public string? ChallanNumber { get; set; }

    public DateOnly? ChallanDate { get; set; }

    public int? ReceivingBranch { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
