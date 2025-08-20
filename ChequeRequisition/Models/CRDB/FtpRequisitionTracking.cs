using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class FtpRequisitionTracking
{
    public int Id { get; set; }

    public int RequisitionId { get; set; }

    public int ImportLogId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual FtpImport ImportLog { get; set; } = null!;
}
