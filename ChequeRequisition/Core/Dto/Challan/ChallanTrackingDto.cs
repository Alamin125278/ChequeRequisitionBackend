namespace ChequeRequisiontService.Core.Dto.Challan;

public class ChallanTrackingDto
{
    public int Id { get; set; }
    public required int ChallanId { get; set; }
    public required int RequisitionItemId { get; set; }
}
