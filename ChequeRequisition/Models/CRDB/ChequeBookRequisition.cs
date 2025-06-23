using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class ChequeBookRequisition
{
    public int Id { get; set; }

    public int BankId { get; set; }

    public int BranchId { get; set; }

    public int? RequestedBy { get; set; }

    public string AccountNo { get; set; } = null!;

    public string RoutingNo { get; set; } = null!;

    public string StartNo { get; set; } = null!;

    public string? EndNo { get; set; }

    public string? ChequeType { get; set; }

    public string ChequePrefix { get; set; } = null!;

    public string MicrNo { get; set; } = null!;

    public string Series { get; set; } = null!;

    public string AccountName { get; set; } = null!;

    public string CusAddress { get; set; } = null!;

    public int BookQty { get; set; }

    public int TransactionCode { get; set; }

    public int Leaves { get; set; }

    public int CourierCode { get; set; }

    public int ReceivingBranchId { get; set; }

    public DateOnly RequestDate { get; set; }

    public int Serverity { get; set; }

    public int Status { get; set; }

    public bool IsDeleted { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Bank Bank { get; set; } = null!;

    public virtual Branch Branch { get; set; } = null!;

    public virtual ICollection<ChallanDetail> ChallanDetails { get; set; } = new List<ChallanDetail>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<FtpRequisitionTracking> FtpRequisitionTrackings { get; set; } = new List<FtpRequisitionTracking>();

    public virtual Branch ReceivingBranch { get; set; } = null!;

    public virtual User? RequestedByNavigation { get; set; }

    public virtual Status StatusNavigation { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
