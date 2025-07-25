﻿using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class ChallanDetail
{
    public int Id { get; set; }

    public int? RequisitionItemId { get; set; }

    public int? ChallanId { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ChequeBookRequisition? RequisitionItem { get; set; }
}
