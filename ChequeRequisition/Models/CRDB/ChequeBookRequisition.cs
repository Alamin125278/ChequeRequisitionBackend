using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class ChequeBookRequisition
{
    public int Id { get; set; }

    public int? BankId { get; set; }

    public int? BranchId { get; set; }

    public int? RequestedBy { get; set; }

    public string? AccountNo { get; set; }

    public string? RouetingNo { get; set; }

    public int? StartNo { get; set; }

    public int? EndNo { get; set; }

    public string? ChequeType { get; set; }

    public string? ChequePrefix { get; set; }

    public string? MicrNo { get; set; }

    public string? Series { get; set; }

    public string? AccountName { get; set; }

    public string? CusAddress { get; set; }

    public int? BookQty { get; set; }

    public int? TransactionCode { get; set; }

    public int? Leaves { get; set; }

    public int? CourierCode { get; set; }

    public int? ReceiviningBranchId { get; set; }

    public DateOnly? RequestDate { get; set; }

    public int? Serverity { get; set; }

    public int? Status { get; set; }

    public bool IsDeleted { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Bank? Bank { get; set; }

    public virtual Branch? Branch { get; set; }

    public virtual ICollection<ChallanDetail> ChallanDetails { get; set; } = new List<ChallanDetail>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Branch? ReceiviningBranch { get; set; }

    public virtual User? RequestedByNavigation { get; set; }

    public virtual Status? StatusNavigation { get; set; }

    public virtual User? UpdatedByNavigation { get; set; }
}
