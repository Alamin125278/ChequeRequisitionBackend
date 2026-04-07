namespace ChequeRequisiontService.Core.Dto.Challan
{
    public class RawChallanItem
    {
        public int ChallanId { get; set; }
        public string ChallanNumber { get; set; }
        public DateTime ChallanDate { get; set; }

        public string CourierName { get; set; }
        public string CourierPhone { get; set; }

        public string BankName { get; set; }
        public string VendorName { get; set; }

        public string HomeBranchName { get; set; }
        public string ChallanBranchName { get; set; }
        public string BranchAddress { get; set; }

        public int? BankId { get; set; }
        public string AgentNum { get; set; }
        public bool? IsAgent { get; set; }
        public DateTime? RequestDate { get; set; }

        public int ItemId { get; set; }
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
        public int? StartNo { get; set; }
        public int? EndNo { get; set; }
        public string ChequeType { get; set; }
        public int? BookQty { get; set; }
        public int? Leaves { get; set; }
        public string Serverity { get; set; }
        public string AccFlag { get; set; }
    }
}
