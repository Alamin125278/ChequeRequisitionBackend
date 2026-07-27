namespace ChequeRequisiontService.Core.Dto.SummaryReport
{
    public class BranchWiseBillDto
    {
        public required int BankId { get; set; }
        public required string DeliveryBranch { get; set; }
        public required bool IsAgent { get; set; }
        public string? DistId { get; set; }
        public int? Sb5 { get; set; }
        public int? Sb10 { get; set; }
        public int? Sb20 { get; set; }
        public int? Sb25 { get; set; }
        public int? Sb50 { get; set; }
        public int? Sba10 { get; set; }
        public int? Msd10 { get; set; }
        public int? Msa10 { get; set; }
        public int? Msa20 { get; set; }
        public int? Msd50 { get; set; }
        public int? Cd5 { get; set; }
        public int? Cd10 { get; set; }
        public int? Cd20 { get; set; }
        public int? Cd25 { get; set; }
        public int? Cd50 { get; set; }
        public int? Cd100 { get; set; }
        public int? Cda25 { get; set; }
        public int? Acd25 { get; set; }
        public int? Acd50 { get; set; }
        public int? Acd100 { get; set; }
        public int? Awcd25 { get; set; }
        public int? Awca20 { get; set; }
        public int? Awca50 { get; set; }
        public int? Msna50 { get; set; }
        public int? Msna100 { get; set; }
        public int? Awca100 { get; set; }
        public int? Sna25 { get; set; }
        public int? Snd25 { get; set; }
        public int? Snd50 { get; set; }
        public int? Snd100 { get; set; }
        public int? Msnd25 { get; set; }
        public int? Po50 { get; set; }
        public int? Po100 { get; set; }
        public int? Ca50 { get; set; }
        public int? Ca100 { get; set; }
        public int? Poa50 { get; set; }
        public int? Poi50 { get; set; }
        public int? Fdr100 { get; set; }
        public int? Fdr50 { get; set; }
        public int? Mtdr25 { get; set; }
        public int? Mtdr50 { get; set; }
        public int? Conv5 { get; set; }
        public int? Conv10 { get; set; }
        public int? Conv20 { get; set; }
        public int? Conv50 { get; set; }
        public int? Islm5 { get; set; }
        public int? Islm10 { get; set; }
        public int? Islm20 { get; set; }
        public int? Islm50 { get; set; }
        public int? Prio10 { get; set; }
        public int? Prio20 { get; set; }
        public int? Prio50 { get; set; }
        public int? Total { get; set; }
        public int ? TotalLeaves { get; set; }
    }
}
