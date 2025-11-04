namespace ChequeRequisiontService.Core.Dto.SummaryReport
{
    public class DailyProductionReportDto
    {
        public required string BankName { get; set; }
        public required int Cd { get; set; }=0;
        public required int Sb { get; set; }=0;
        public required int Po { get; set; }=0;
        public required int A4 { get; set; }=0;
        public required int MtdrFdr { get; set; }=0;
        public required int TotalLeaves { get; set; }=0;
        public required int TotalBooks { get; set; } = 0;
        public required int TotalBranch { get; set; } = 0;
    }
}
