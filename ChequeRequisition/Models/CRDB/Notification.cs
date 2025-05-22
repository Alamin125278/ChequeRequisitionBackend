using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class Notification
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string? Url { get; set; }

    public bool IsRead { get; set; }

    public string? Type { get; set; }

    public string? ModuleName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ReadAt { get; set; }

    public virtual User User { get; set; } = null!;
}
