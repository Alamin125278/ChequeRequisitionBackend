using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class MmblBranch
{
    public string? Id { get; set; }

    public byte BankId { get; set; }

    public string BranchName { get; set; } = null!;

    public string BranchCode { get; set; } = null!;

    public string BranchEmail { get; set; } = null!;

    public string BranchAddress { get; set; } = null!;

    public byte BranchPhone { get; set; }

    public string RoutingNo { get; set; } = null!;

    public byte IsActive { get; set; }

    public byte IsDeleted { get; set; }

    public byte CreatedBy { get; set; }
}
