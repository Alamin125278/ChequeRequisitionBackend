namespace ChequeRequisiontService.Core.Dto.Ftp;

public class FtpImportLogDto
{
    public int BankId { get; set; }
    public required string Filename { get; set; }
    public DateTime ImportedAt { get; set; }
    public DateTime? ClearedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
