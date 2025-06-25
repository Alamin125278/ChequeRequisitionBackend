using ChequeRequisiontService.Core.Dto.Bank;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChequeRequisiontService.Core.Dto.Branch
{
    public class BranchDto
    {
      
        public int Id { get; set; }
        public required int BankId { get; set; }
        public required string BranchName { get; set; }
        public required string BranchCode { get; set; }
        public required string BranchEmail { get; set; }
        public required string BranchPhone { get; set; }
        public required string BranchAddress { get; set; }
        public required string RoutingNo { get; set; }
        public bool IsActive { get; set; } = true;
        public BankDto? Bank { get; set; }
        public string BankName => Bank?.BankName ?? string.Empty;
    }
}
