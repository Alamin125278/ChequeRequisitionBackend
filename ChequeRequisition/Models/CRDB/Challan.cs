﻿using System;
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

    public virtual ICollection<ChallanDetail> ChallanDetails { get; set; } = new List<ChallanDetail>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Branch? ReceivingBranchNavigation { get; set; }

    public virtual User? UpdatedByNavigation { get; set; }
}
