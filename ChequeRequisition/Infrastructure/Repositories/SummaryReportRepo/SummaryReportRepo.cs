using ChequeRequisiontService.Core.Dto.SummaryReport;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.DbContexts;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace ChequeRequisiontService.Infrastructure.Repositories.SummaryReportRepo;

public class SummaryReportRepo(CRDBContext cRDBContext) : ISummaryReport

{
    private CRDBContext _cRDBContext = cRDBContext;
    public async Task<IEnumerable<SummaryReportDto>> GetSummaryReportAsync(int BankId, DateOnly fromDate, DateOnly toDate,int Severity,bool AgentType, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = from challan in _cRDBContext.Challans.AsNoTracking()
                        join tracking in _cRDBContext.ChallanDetails on challan.Id equals tracking.ChallanId
                        join requisition in _cRDBContext.ChequeBookRequisitions on tracking.RequisitionItemId equals requisition.Id
                        join homeBranch in _cRDBContext.Branches on requisition.BranchId equals homeBranch.Id
                        join deliveryBranch in _cRDBContext.Branches on requisition.ReceivingBranchId equals deliveryBranch.Id
                        where  requisition.IsAgent==AgentType && requisition.Serverity==Severity && requisition.BankId == BankId && challan.ChallanDate >= fromDate && challan.ChallanDate <= toDate
                        group new { requisition, challan, homeBranch, deliveryBranch } by new
                        {
                            challan.ChallanNumber,
                            challan.ChallanDate,
                            HomeBranchName = homeBranch.BranchName,
                            DeliveryBranchName = deliveryBranch.BranchName,
                            isAgent =requisition.IsAgent
                        } into g
                        orderby g.Key.ChallanNumber
                        select new SummaryReportDto
                        {
                            HomeBranch = g.Key.HomeBranchName,
                            DeliveryBranch = g.Key.DeliveryBranchName,
                            ChallanNo = g.Key.ChallanNumber,
                            ChallanDate = (DateOnly)(g.Key.ChallanDate),
                            IsAgent =g.Key.isAgent??false,

                            Sb10 = g.Sum(x => x.requisition.ChequeType == "Savings" && x.requisition.Leaves == 10 ? x.requisition.BookQty : 0),
                            Sb20 = g.Sum(x => x.requisition.ChequeType == "Savings" && x.requisition.Leaves == 20 ? x.requisition.BookQty : 0),
                            Sb25 = g.Sum(x => x.requisition.ChequeType == "Savings" && x.requisition.Leaves == 25 ? x.requisition.BookQty : 0),

                            Cd10 = g.Sum(x => x.requisition.ChequeType == "Current" && x.requisition.Leaves == 10 ? x.requisition.BookQty : 0),
                            Cd25 = g.Sum(x => x.requisition.ChequeType == "Current" && x.requisition.Leaves == 25 ? x.requisition.BookQty : 0),
                            Cd50 = g.Sum(x => x.requisition.ChequeType == "Current" && x.requisition.Leaves == 50 ? x.requisition.BookQty : 0),
                            Cd100 = g.Sum(x => x.requisition.ChequeType == "Current" && x.requisition.Leaves == 100 ? x.requisition.BookQty : 0),

                            Po50 = g.Sum(x => x.requisition.ChequeType == "Payment Order" && x.requisition.Leaves == 50 ? x.requisition.BookQty : 0),
                            Po100 = g.Sum(x => x.requisition.ChequeType == "Payment Order" && x.requisition.Leaves == 100 ? x.requisition.BookQty : 0),
                            Ca50 = g.Sum(x => x.requisition.ChequeType == "Cash Credit" && x.requisition.Leaves == 50 ? x.requisition.BookQty : 0),
                            Ca100 = g.Sum(x => x.requisition.ChequeType == "Cash Credit" && x.requisition.Leaves == 100 ? x.requisition.BookQty : 0),
                            Total = g.Sum(x => x.requisition.BookQty)
                        };
            var data = await query.ToListAsync(cancellationToken);
            return data;
        }
        catch(DbException ex)
        {
            throw new Exception("Database update error: " + (ex.InnerException?.Message ?? ex.Message), ex);
        }

    }
}
