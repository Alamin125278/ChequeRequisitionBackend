namespace ChequeRequisiontService.Core.Dto.SummaryReport;

public class SummaryReportDto
{
    public required string HomeBranch { get; set; }
    public required string DeliveryBranch { get; set; }
    public required string ChallanNo { get; set; }
    public required DateOnly ChallanDate { get; set; }
    public required bool IsAgent { get; set; }
    public int? Sb10 { get; set; }
    public  int? Sb20 { get; set; }
    public  int? Sb25 { get; set; }
    public  int? Cd10 { get; set; }
    public  int? Cd25 { get; set; }
    public  int? Cd50 { get; set; }
    public  int? Cd100 { get; set; }
    public  int? Po50 { get; set; }
    public  int? Po100 { get; set; }
    public  int? Ca50 { get; set; }
    public  int? Ca100 { get; set; }
    public  int? Total { get; set; }
}
