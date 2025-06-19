namespace ChequeRequisiontService.Core.Dto.Ftp;

public class FtpFileLogDto
{
    int Id { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string LogLevel { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Exception { get; set; } = null;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
