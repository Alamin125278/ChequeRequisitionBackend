namespace ChequeRequisiontService.Core.Dto.SummaryReport;

public class ConsumptionReportDto
{

    public DateOnly? RequestDate { get; set; } = null;
    public int? Sb10Books { get; set; }
    public int? Sb10Leaves { get; set; }
    public int? Sb20Books { get; set; }
    public int? Sb20Leaves { get; set; }
    public int? Sb25Books { get; set; }
    public int? Sb25Leaves { get; set; }
    public int? Sba10Books { get; set; }
    public int? Sba10Leaves { get; set; }
    public int? Msd10Books { get; set; }
    public int? Msd10Leaves { get; set; }
    public int? Cd10Books { get; set; }
    public int? Cd10Leaves { get; set; }
    public int? Cd25Books { get; set; }
    public int? Cd25Leaves { get; set; }
    public int? Cd50Books { get; set; }
    public int? Cd50Leaves { get; set; }
    public int? Cd100Books { get; set; }
    public int? Cd100Leaves { get; set; }
    public int? Cda25Books { get; set; }
    public int? Cda25Leaves { get; set; }
    public int? Awcd25Books { get; set; }
    public int? Awcd25Leaves { get; set; }
    public int? Sna25Books { get; set; }
    public int? Sna25Leaves { get; set; }
    public int? Msnd25Books { get; set; }
    public int? Msnd25Leaves { get; set; }
    public int? Po50Books { get; set; }
    public int? Po50Leaves { get; set; }
    public int? Po100Books { get; set; }
    public int? Po100Leaves { get; set; }
    public int? Cc50Books { get; set; }
    public int? Cc50Leaves { get; set; }
    public int? Cc100Books { get; set; }
    public int? Cc100Leaves { get; set; }
    public int? Poa50Books { get; set; }
    public int? Poa50Leaves { get; set; }
    public int? Poi50Books { get; set; }
    public int? Poi50Leaves { get; set; }
    public int? Fdr100Books { get; set; }
    public int? Fdr100Leaves { get; set; }
    public int? Fdr50Books { get; set; }
    public int? Fdr50Leaves { get; set; }
    public int? Mtdr25Books { get; set; }
    public int? Mtdr25Leaves { get; set; }
    public int? Mtdr50Books { get; set; }
    public int? Mtdr50Leaves { get; set; }

    public int? TotalBooks { get; set; }
    public int? TotalLeaves { get; set; }
}

