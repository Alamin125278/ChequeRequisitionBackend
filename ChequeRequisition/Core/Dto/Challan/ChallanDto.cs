using DocumentFormat.OpenXml.Office2010.Excel;

namespace ChequeRequisiontService.Core.Dto.Challan;

public class ChallanDto
{
    public int Id { get; set; }
    public string? ChallanNumber { get; set; }
    public DateOnly? ChallanDate { get; set; }
    public int? ReceivingBranch { get; set; }

    public string? BankName { get; set; }
    public string? ReceivingBranchName { get; set; }
    public string? VendorName { get; set; }
    public string? CourierName { get; set; }
    public int? RequisitionCount { get; set; } = 0;
}
