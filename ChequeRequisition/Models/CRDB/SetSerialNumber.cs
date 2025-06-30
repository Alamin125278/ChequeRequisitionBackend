using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class SetSerialNumber
{
    public int Id { get; set; }

    public int BankId { get; set; }

    public string ChequeType { get; set; } = null!;

    public int TrCode { get; set; }

    public string StartingNo { get; set; } = null!;

    public string EndingNo { get; set; } = null!;

    public string EndLimit { get; set; } = null!;

    public string Series { get; set; } = null!;

    public bool IsDelete { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Bank Bank { get; set; } = null!;

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? UpdatedByNavigation { get; set; }
}
