using ChequeRequisiontService.Core.Dto.Bank;
using ChequeRequisiontService.Core.Dto.Branch;
using ChequeRequisiontService.Core.Dto.Status;
using ChequeRequisiontService.Core.Dto.User;
using Microsoft.VisualBasic;
using System.Numerics;

namespace ChequeRequisiontService.Core.Dto.Requisition
{
    public class RequisitionDto
    {
        public int Id { get; set; }
        public required int BankId { get; set; }
        public required int BranchId { get; set; }
        public required string AccountNo { get; set; }
        public required string RoutingNo { get; set; }
        public required string StartNo { get; set; }
        public required string EndNo { get; set; }
        public required string ChequeType { get; set; } 
        public required string ChequePrefix { get; set; }
        public required string MicrNo { get; set; }
        public required string Series { get; set; }
        public required string AccountName { get; set; }
        public required string CusAddress { get; set; }
        public required int BookQty { get; set; }
        public required int TransactionCode { get; set; }
        public required int Leaves { get; set; }
        public required string CourierCode { get; set; }
        public required int ReceivingBranchId { get; set; }
        public  string RequestDate { get; set; }
        public required int Serverity { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public string? AgentNum { get; set; }
        public string? ChallanNumber { get; set; }
        public bool IsAgent { get; set; } = false;

        public required string AccFlag { get; set; }

        public required int Status { get; set; } // Assuming 1 is the default status for a new requisition
        public bool IsDeleted { get; set; } = false;

        
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
        public string StatusName { get; set; }

        public string RequestName { get; set; }
        public string ReceivingBranchName { get; set; }
        public string ReceivingBranchCode { get; set; }

    }
}
