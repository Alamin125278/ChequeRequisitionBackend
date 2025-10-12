namespace ChequeRequisiontService.Core.Dto.SummaryReport;

public class SummaryReportDto
{
    public string? HomeBranch { get; set; }
    public required string DeliveryBranch { get; set; }
    public required string ChallanNo { get; set; }
    public required DateOnly ChallanDate { get; set; }
    public required bool IsAgent { get; set; }
    public int? Sb10 { get; set; }
    public  int? Sb20 { get; set; }
    public  int? Sb25 { get; set; }
    public  int? Sba10 { get; set; }
    public  int? Msd10 { get; set; }
    public  int? Cd10 { get; set; }
    public  int? Cd25 { get; set; }
    public  int? Cd50 { get; set; }
    public  int? Cd100 { get; set; }
    public  int? Cda25 { get; set; }
    public  int? Awcd25 { get; set; }
    public  int? Sna25 { get; set; }
    public  int? Msnd25 { get; set; }
    public  int? Po50 { get; set; }
    public  int? Po100 { get; set; }
    public  int? Ca50 { get; set; }
    public  int? Ca100 { get; set; }
    public  int? Poa50 { get; set; }
    public  int? Poi50 { get; set; }
    public  int? Fdr100 { get; set; }
    public  int? Fdr50 { get; set; }
    public  int? Mtdr25 { get; set; }
    public  int? Mtdr50 { get; set; }
    public  int? Total { get; set; }
    public string? CourierName { get; set; } = null;
    public DateOnly? RequestDate { get; set; } = null;
    }
