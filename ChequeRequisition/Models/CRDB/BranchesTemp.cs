using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class BranchesTemp
{
    public string? Id { get; set; }

    public byte BankId { get; set; }

    public string BranchName { get; set; } = null!;

    public string BranchCode { get; set; } = null!;

    public string BranchEmail { get; set; } = null!;

    public string BranchAddress { get; set; } = null!;

    public string BranchPhone { get; set; } = null!;

    public int RoutingNo { get; set; }

    public byte IsActive { get; set; }

    public byte IsDeleted { get; set; }

    public byte CreatedBy { get; set; }
}
