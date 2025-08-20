using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class FtpImport
{
    public int Id { get; set; }

    public int? BankId { get; set; }

    public string? FileName { get; set; }

    public DateTime? ImportedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<FtpRequisitionTracking> FtpRequisitionTrackings { get; set; } = new List<FtpRequisitionTracking>();
}
