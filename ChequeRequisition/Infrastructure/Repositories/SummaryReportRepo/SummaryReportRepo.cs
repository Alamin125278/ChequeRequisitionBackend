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
    public async Task<IEnumerable<SummaryReportDto>> GetSummaryReportAsync(int BankId, DateOnly fromDate, DateOnly toDate, int Severity, bool AgentType, CancellationToken cancellationToken = default)
    {
        try
        {
            // Step 1: প্রথমে filtered requisitions নিন
            var filteredRequisitions = _cRDBContext.ChequeBookRequisitions
                .AsNoTracking()
                .Where(r => r.IsAgent == AgentType &&
                           r.BankId == BankId &&
                           r.RequestDate >= fromDate &&
                           r.RequestDate <= toDate)
                .Select(r => new
                {
                    r.Id,
                    r.BranchId,
                    r.ReceivingBranchId,
                    r.CourierCode,
                    r.RequestDate,
                    r.ChequeType,
                    r.Leaves,
                    r.BookQty,
                    r.IsAgent
                });

            // Step 2: এখন filtered data দিয়ে join করুন
            var query = from requisition in filteredRequisitions
                        join tracking in _cRDBContext.ChallanDetails.AsNoTracking()
                            on requisition.Id equals tracking.RequisitionItemId
                        join challan in _cRDBContext.Challans.AsNoTracking()
                            on tracking.ChallanId equals challan.Id
                        join homeBranch in _cRDBContext.Branches.AsNoTracking()
                            on requisition.BranchId equals homeBranch.Id
                        join deliveryBranch in _cRDBContext.Branches.AsNoTracking()
                            on requisition.ReceivingBranchId equals deliveryBranch.Id
                        join courier in _cRDBContext.Couriers.AsNoTracking()
                            on requisition.CourierCode equals courier.CourierCode
                        group new { requisition, challan, homeBranch, deliveryBranch, courier } by new
                        {
                            challan.ChallanNumber,
                            challan.ChallanDate,
                            HomeBranchName = homeBranch.BranchName,
                            DeliveryBranchName = deliveryBranch.BranchName,
                            isAgent = requisition.IsAgent
                        } into g
                        orderby g.Key.ChallanNumber
                        select new SummaryReportDto
                        {
                            HomeBranch = g.Key.HomeBranchName,
                            DeliveryBranch = g.Key.DeliveryBranchName,
                            ChallanNo = g.Key.ChallanNumber,
                            ChallanDate = (DateOnly)(g.Key.ChallanDate),
                            IsAgent = g.Key.isAgent ?? false,
                            CourierName = g.Select(x => x.courier.CourierName).FirstOrDefault(),
                            RequestDate = g.Select(x => x.requisition.RequestDate).FirstOrDefault(),

                            // Savings
                            Sb10 = g.Sum(x => x.requisition.ChequeType == "Savings" && x.requisition.Leaves == 10 ? x.requisition.BookQty : 0),
                            Sb20 = g.Sum(x => x.requisition.ChequeType == "Savings" && x.requisition.Leaves == 20 ? x.requisition.BookQty : 0),
                            Sb25 = g.Sum(x => x.requisition.ChequeType == "Savings" && x.requisition.Leaves == 25 ? x.requisition.BookQty : 0),

                            // Current
                            Cd10 = g.Sum(x => x.requisition.ChequeType == "Current" && x.requisition.Leaves == 10 ? x.requisition.BookQty : 0),
                            Cd25 = g.Sum(x => x.requisition.ChequeType == "Current" && x.requisition.Leaves == 25 ? x.requisition.BookQty : 0),
                            Cd50 = g.Sum(x => x.requisition.ChequeType == "Current" && x.requisition.Leaves == 50 ? x.requisition.BookQty : 0),
                            Cd100 = g.Sum(x => x.requisition.ChequeType == "Current" && x.requisition.Leaves == 100 ? x.requisition.BookQty : 0),

                            // Payment Order
                            Po50 = g.Sum(x => x.requisition.ChequeType == "Payment Order" && x.requisition.Leaves == 50 ? x.requisition.BookQty : 0),
                            Po100 = g.Sum(x => x.requisition.ChequeType == "Payment Order" && x.requisition.Leaves == 100 ? x.requisition.BookQty : 0),

                            // Cash Credit
                            Ca50 = g.Sum(x => x.requisition.ChequeType == "Cash Credit" && x.requisition.Leaves == 50 ? x.requisition.BookQty : 0),
                            Ca100 = g.Sum(x => x.requisition.ChequeType == "Cash Credit" && x.requisition.Leaves == 100 ? x.requisition.BookQty : 0),

                            // Bengal Bank - Savings Types
                            Sba10 = g.Sum(x => x.requisition.ChequeType == "SBA" && x.requisition.Leaves == 10 ? x.requisition.BookQty : 0),
                            Msd10 = g.Sum(x => x.requisition.ChequeType == "MSD" && x.requisition.Leaves == 10 ? x.requisition.BookQty : 0),

                            // Bengal Bank - Current Types
                            Cda25 = g.Sum(x => x.requisition.ChequeType == "CDA" && x.requisition.Leaves == 25 ? x.requisition.BookQty : 0),
                            Awcd25 = g.Sum(x => x.requisition.ChequeType == "AWCD" && x.requisition.Leaves == 25 ? x.requisition.BookQty : 0),
                            Sna25 = g.Sum(x => x.requisition.ChequeType == "SNA" && x.requisition.Leaves == 25 ? x.requisition.BookQty : 0),
                            Msnd25 = g.Sum(x => x.requisition.ChequeType == "MSND" && x.requisition.Leaves == 25 ? x.requisition.BookQty : 0),

                            // Bengal Bank - Payment Order
                            Poa50 = g.Sum(x => x.requisition.ChequeType == "POA" && x.requisition.Leaves == 50 ? x.requisition.BookQty : 0),
                            Poi50 = g.Sum(x => x.requisition.ChequeType == "POI" && x.requisition.Leaves == 50 ? x.requisition.BookQty : 0),

                            // Bengal Bank - FDR & MTDR
                            Fdr50 = g.Sum(x => x.requisition.ChequeType == "FDR" && x.requisition.Leaves == 50 ? x.requisition.BookQty : 0),
                            Fdr100 = g.Sum(x => x.requisition.ChequeType == "FDR" && x.requisition.Leaves == 100 ? x.requisition.BookQty : 0),
                            Mtdr25 = g.Sum(x => x.requisition.ChequeType == "MTDR" && x.requisition.Leaves == 25 ? x.requisition.BookQty : 0),
                            Mtdr50 = g.Sum(x => x.requisition.ChequeType == "MTDR" && x.requisition.Leaves == 50 ? x.requisition.BookQty : 0),

                            Total = g.Sum(x => x.requisition.BookQty)
                        };

            // Command timeout set করুন
            _cRDBContext.Database.SetCommandTimeout(120); // 3 minutes

            var data = await query.ToListAsync(cancellationToken);
            return data;
        }
        catch (DbException ex)
        {
            throw new Exception("Database error: " + (ex.InnerException?.Message ?? ex.Message), ex);
        }
    }
    public async Task<IEnumerable<SummaryReportDto>> GetCourierSummaryReportAsync(int BankId, DateOnly fromDate, DateOnly toDate,int Severity,bool AgentType, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = from challan in _cRDBContext.Challans.AsNoTracking()
                        join tracking in _cRDBContext.ChallanDetails on challan.Id equals tracking.ChallanId
                        join requisition in _cRDBContext.ChequeBookRequisitions on tracking.RequisitionItemId equals requisition.Id
                        join courier in _cRDBContext.Couriers on requisition.CourierCode equals courier.CourierCode
                        join deliveryBranch in _cRDBContext.Branches on requisition.ReceivingBranchId equals deliveryBranch.Id
                        where  requisition.IsAgent==AgentType && requisition.Serverity==Severity && requisition.BankId == BankId && challan.ChallanDate >= fromDate && challan.ChallanDate <= toDate
                        group new { requisition, challan, deliveryBranch, courier } by new
                        {
                            challan.ChallanNumber,
                            challan.ChallanDate,
                            DeliveryBranchName = deliveryBranch.BranchName,
                            isAgent =requisition.IsAgent
                        } into g
                        orderby g.Key.ChallanNumber
                        select new SummaryReportDto
                        {
                            DeliveryBranch = g.Key.DeliveryBranchName,
                            ChallanNo = g.Key.ChallanNumber,
                            ChallanDate = (DateOnly)(g.Key.ChallanDate),
                            IsAgent =g.Key.isAgent??false,
                            CourierName= g.Select(x => x.courier.CourierName).FirstOrDefault(),
                            RequestDate= g.Select(x => x.requisition.RequestDate).FirstOrDefault(),
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

                            //Bengul Bank
                            Sba10 = g.Sum(x => x.requisition.ChequeType == "SBA" && x.requisition.Leaves == 10 ? x.requisition.BookQty : 0),
                            Msd10 = g.Sum(x => x.requisition.ChequeType == "MSD" && x.requisition.Leaves == 10 ? x.requisition.BookQty : 0),
                            
                            Cda25 = g.Sum(x => x.requisition.ChequeType == "CDA" && x.requisition.Leaves == 25 ? x.requisition.BookQty : 0),
                            Awcd25 = g.Sum(x => x.requisition.ChequeType == "AWCD" && x.requisition.Leaves == 25 ? x.requisition.BookQty : 0),
                            Sna25 = g.Sum(x => x.requisition.ChequeType == "SNA" && x.requisition.Leaves == 25 ? x.requisition.BookQty : 0),
                            Msnd25 = g.Sum(x => x.requisition.ChequeType == "MSND" && x.requisition.Leaves == 25 ? x.requisition.BookQty : 0),
                            
                            Poa50 = g.Sum(x => x.requisition.ChequeType == "POA" && x.requisition.Leaves == 50 ? x.requisition.BookQty : 0),
                            Poi50 = g.Sum(x => x.requisition.ChequeType == "POI" && x.requisition.Leaves == 50 ? x.requisition.BookQty : 0),

                            Fdr50 = g.Sum(x => x.requisition.ChequeType == "FDR" && x.requisition.Leaves == 50 ? x.requisition.BookQty : 0),
                            Fdr100 = g.Sum(x => x.requisition.ChequeType == "FDR" && x.requisition.Leaves == 100 ? x.requisition.BookQty : 0),
                            Mtdr25 = g.Sum(x => x.requisition.ChequeType == "MTDR" && x.requisition.Leaves == 25 ? x.requisition.BookQty : 0),
                            Mtdr50 = g.Sum(x => x.requisition.ChequeType == "MTDR" && x.requisition.Leaves == 50 ? x.requisition.BookQty : 0),

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
