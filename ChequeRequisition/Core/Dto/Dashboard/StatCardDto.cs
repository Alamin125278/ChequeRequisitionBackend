namespace ChequeRequisiontService.Core.Dto.Dashboard;

public class StatCardDto
{
    public int? TotalRequisition { get; set; }
    public int? OrderedRequisition { get; set; }
    public int? ProcessingRequisition { get; set; }
    public int? DispatchedRequisition { get; set; }
    public int? DeliveredRequisition { get; set; }
}
