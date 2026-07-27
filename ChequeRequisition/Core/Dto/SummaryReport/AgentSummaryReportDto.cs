namespace ChequeRequisiontService.Core.Dto.SummaryReport
{
    public class AgentSummaryReportDto
    {
        public required int BankId { get; set; }
        public required string DeliveryBranch { get; set; }
        public required string CourierName { get; set; }
        public string? DistId { get; set; }
        public int? Msa10 { get; set; }
        public int? Msa20 { get; set; }
        public int? Awca20 { get; set; }
        public int? Awca50 { get; set; }
        public int? Awca100 { get; set; }
        public int? Po50 { get; set; }
        public int? Total { get; set; }
        public DateOnly? RequestDate { get; set; } = null;
    }
}
