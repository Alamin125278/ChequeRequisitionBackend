namespace ChequeRequisiontService.Core.Dto.BranchDto
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
        public bool IsDeleted { get; set; } = false;
    }
}
