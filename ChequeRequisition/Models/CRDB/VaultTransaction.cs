using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class VaultTransaction
{
    public int Id { get; set; }

    public int? BankId { get; set; }

    public int? CheckbookTypeId { get; set; }

    public int? Quantity { get; set; }

    public DateOnly? VaultInDate { get; set; }

    public string? Remarks { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
