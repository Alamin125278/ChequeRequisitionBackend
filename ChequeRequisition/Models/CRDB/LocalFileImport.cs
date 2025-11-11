using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class LocalFileImport
{
    public int Id { get; set; }

    public int BankId { get; set; }

    public string FileName { get; set; } = null!;

    public DateOnly? ImportedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int CreatedBy { get; set; }
}
