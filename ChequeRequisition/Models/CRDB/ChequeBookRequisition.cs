using System;
using System.Collections.Generic;

namespace ChequeRequisiontService.Models.CRDB;

public partial class ChequeBookRequisition
{
    public int Id { get; set; }

    public string BankName { get; set; } = null!;

    public string BranchName { get; set; } = null!;

    public int? RequestedBy { get; set; }

    public long AccountNo { get; set; }

    public int RoutingNo { get; set; }

    public int StartNo { get; set; }

    public int? EndNo { get; set; }

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

    public string ReceivingBranchName { get; set; } = null!;

    public DateOnly RequestDate { get; set; }

    public int Serverity { get; set; }

    public int Status { get; set; }

    public bool IsDeleted { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ChallanDetail> ChallanDetails { get; set; } = new List<ChallanDetail>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<FtpRequisitionTracking> FtpRequisitionTrackings { get; set; } = new List<FtpRequisitionTracking>();

    public virtual User? RequestedByNavigation { get; set; }

    public virtual Status StatusNavigation { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
