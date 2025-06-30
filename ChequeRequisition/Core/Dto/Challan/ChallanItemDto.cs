namespace ChequeRequisiontService.Core.Dto.Challan;


public class ChallanItemDto
{
    public required string AccountNo { get; set; }
    public required string AccountName { get; set; }
    public required string StartNo { get; set; }
    public required string EndNo { get; set; }
    public required string ChequeType { get; set; }
    public required int BookQty { get; set; }
    public required int Leaves { get; set; }
    public required int Serverity { get; set; }
    public required string BranchName { get; set; }
}
