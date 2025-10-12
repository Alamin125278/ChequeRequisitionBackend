namespace ChequeRequisiontService.Core.Dto.ManageSerial;

public class ManageSerialDto
{
    public int Id { get; set; }
    public int BankId { get; set; }
    public required string ChequeType { get; set; }
    public required int TrCode { get; set; }
    public required int Lvs { get; set; }
    public required string StartingNo { get; set; }
    public required string EndingNo { get; set; }

    public required string EndLimit { get; set; }
    public required string Series { get; set; }

}
