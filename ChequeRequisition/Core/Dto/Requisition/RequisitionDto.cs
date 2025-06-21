using Microsoft.VisualBasic;
using System.Numerics;

namespace ChequeRequisiontService.Core.Dto.Requisition
{
    public class RequisitionDto
    {
        public int Id { get; set; }
        public required string BankName { get; set; }
        public required string BranchName { get; set; }
        public required BigInteger AccountNo { get; set; }
        public required int RoutingNo { get; set; }
        public required int StartNo { get; set; }
        public required int EndNo { get; set; }
        public required string ChequeType { get; set; } 
        public required string ChequePrefix { get; set; }
        public required string MicrNo { get; set; }
        public required string Series { get; set; }
        public required string AccountName { get; set; }
        public required string CusAddress { get; set; }
        public required int BookQty { get; set; }
        public required int TransactionCode { get; set; }
        public required int Leaves { get; set; }
        public required int CourierCode { get; set; }
        public required string ReceivingBranchName { get; set; }
        public required DateOnly RequestDate { get; set; }
        public required int Serverity { get; set; }
        public required int Status { get; set; } // Assuming 1 is the default status for a new requisition
        public bool IsDeleted { get; set; } = false;
    }
}
