namespace ChequeRequisiontService.Core.Dto.SummaryReport
{
    public class AgentReceiptDto
    {
        public string DeliveryBranch { get; set; } = string.Empty;
        public string DistId { get; set; } = string.Empty;
        public string ChallanNo { get; set; } = string.Empty;

        public List<ChequeBookItemDto> Items { get; set; } = new();

        public int TotalBookQty { get; set; }
    }

    public class ChequeBookItemDto
    {
        public string AccountNo { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string StartNo { get; set; } = string.Empty;
        public int BookQty { get; set; }
        public int Leaves { get; set; }
        public string EndNo { get; set; } = string.Empty;
        public string AccType { get; set; } = string.Empty;
    }
}
