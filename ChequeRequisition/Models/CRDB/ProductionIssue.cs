using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class ProductionIssue
{
    public int Id { get; set; }

    public int? BankId { get; set; }

    public int? CheckbookTypeId { get; set; }

    public int? IssueQty { get; set; }

    public DateOnly? IssueDate { get; set; }

    public string? ReferenceNo { get; set; }

    public string? Remarks { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
