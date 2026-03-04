using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class BankChequeRandomSerial
{
    public long Id { get; set; }

    public int BankId { get; set; }

    public string ChequeSerial { get; set; } = null!;

    public string GeneratedNumber { get; set; } = null!;

    public DateOnly CreatedDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public long CreatedBy { get; set; }
}
