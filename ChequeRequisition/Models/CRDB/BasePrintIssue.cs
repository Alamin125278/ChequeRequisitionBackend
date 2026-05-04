using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class BasePrintIssue
{
    public int Id { get; set; }

    public int? BankId { get; set; }

    public int? CbsId { get; set; }

    public string? IssueNo { get; set; }

    public DateOnly? IssueDate { get; set; }

    public int? IssueQuantity { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
