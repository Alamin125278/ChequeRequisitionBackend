using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class SetSerialNumber
{
    public int Id { get; set; }

    public int? BankId { get; set; }

    public string? ChequeType { get; set; }

    public int? TrCode { get; set; }

    public int? StartingNo { get; set; }

    public int? EndNo { get; set; }

    public int? EndingNo { get; set; }

    public string? Series { get; set; }

    public bool IsDelete { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Bank? Bank { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? UpdatedByNavigation { get; set; }
}
