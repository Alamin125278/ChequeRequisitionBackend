using DocumentFormat.OpenXml.Office2010.Excel;

namespace ChequeRequisiontService.Core.Dto.Challan;

public class ChallanDto
{
    public int Id { get; set; }
    public string? ChallanNumber { get; set; }
    public required DateOnly ChallanDate { get; set; }
    public required int ReceivingBranch { get; set; }
}
