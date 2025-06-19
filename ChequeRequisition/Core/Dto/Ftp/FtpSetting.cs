namespace ChequeRequisiontService.Core.Dto.Ftp;

public class FtpSetting
{
    public required string BankName { get; set; }
    public required string Host { get; set; }
    public required string User { get; set; }
    public required string Password { get; set; }
    public required string RemotePath { get; set; }
}
