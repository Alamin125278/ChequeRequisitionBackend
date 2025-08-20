namespace ChequeRequisiontService.Core.Dto.Challan
{
    public class ChallanExportDto
    {
        public required string BankName { get; set; }
        public required string ReceivingBranchName { get; set; }
        public required string ChallanDate { get; set; }
        public required string VendorName { get; set; }
        public required string CourierName { get; set; }
        public required string CourierPhone { get; set; }
        public required string ChallanNumber { get; set; }
        public required bool IsAgent { get; set; }
        public string? AgentNum { get; set; }
        public string? CusAddress { get; set; }
        public required List<ChallanItemDto> Items { get; set; }  // 👈 multiple requisitions
    }

}
