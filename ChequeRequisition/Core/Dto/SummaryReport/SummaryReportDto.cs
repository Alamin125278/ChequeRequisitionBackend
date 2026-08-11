namespace ChequeRequisiontService.Core.Dto.SummaryReport;

public class SummaryReportDto
{
    public string? HomeBranch { get; set; }
    public required int BankId { get; set; }
    public required string DeliveryBranch { get; set; }
    public required string ChallanNo { get; set; }
    public required DateOnly ChallanDate { get; set; }
    public required bool IsAgent { get; set; }
    public required  string BranchAddress { get; set; }
    public required string BranchPhone { get; set; }
    public int? Consb10 { get; set; }
    public int? Islmsb10 { get; set; }
    public  int? Consb20 { get; set; }
    public  int? Islmsb20 { get; set; }
    public  int? Consb25 { get; set; }
    public  int? Islmsb25 { get; set; }
    public  int? Consba10 { get; set; }
    public  int? Conmsd10 { get; set; }
    public  int? Conmsd50 { get; set; }
    public  int? Conmsa10 { get; set; }
    public  int? Conmsa20 { get; set; }
    public  int? Concd10 { get; set; }
    public  int? Concd20 { get; set; }
    public  int? Concd25 { get; set; }
    public  int? Islmcd25 { get; set; }
    public  int? Concd50 { get; set; }
    public  int? Islmcd50 { get; set; }
    public  int? Concd100 { get; set; }
    public  int? Concda25 { get; set; }
    public int? Conacd25 { get; set; }
    public int? Conacd50 { get; set; }
    public int? Conacd100 { get; set; }
    public  int? Conawcd25 { get; set; }
    public  int? Conawca20 { get; set; }
    public  int? Conawca50 { get; set; }
    public  int? Conawca100 { get; set; }
    //public  int? Msna50 { get; set; }
    //public  int? Msna100 { get; set; }
    public  int? Consna25 { get; set; }
    public  int? Consnd25 { get; set; }
    public  int? Consnd50 { get; set; }
    public  int? Consnd100 { get; set; }
    public  int? Conmsnd25 { get; set; }
    public  int? Conpo50 { get; set; }
    public  int? Conpo100 { get; set; }
    public  int? Conca50 { get; set; }
    public  int? Conca100 { get; set; }
    public  int? Conpoa50 { get; set; }
    public  int? Conpoi50 { get; set; }
    public  int? Confdr100 { get; set; }
    public  int? Confdr50 { get; set; }
    public  int? Conmtdr25 { get; set; }
    public  int? Conmtdr50 { get; set; }
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
    public  int? Total { get; set; }
    public string? CourierName { get; set; } = null;
    public DateOnly? RequestDate { get; set; } = null;
    }
