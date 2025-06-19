using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class FtpFileShareLog
{
    public int Id { get; set; }

    public string? BankName { get; set; }

    public string? LogLevel { get; set; }

    public string? Message { get; set; }

    public string? Exception { get; set; }

    public DateTime? CreatedAt { get; set; }
}
