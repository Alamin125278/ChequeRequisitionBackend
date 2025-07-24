namespace ChequeRequisiontService.Core.Dto.Dashboard;

public class OrderTrackingDto
{
    public required string Label { get; set; }
    public int? Count { get; set; } = 0;
}
