using ChequeRequisiontService.Core.Dto.SummaryReport;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.DbContexts;
using ChequeRequisiontService.Endpoints.SummaryReport.ConsumptionReport;
using ChequeRequisiontService.Models.CRDB;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace ChequeRequisiontService.Infrastructure.Repositories.SummaryReportRepo;

public class SummaryReportRepo(CRDBContext cRDBContext) : ISummaryReport

{
    private CRDBContext _cRDBContext = cRDBContext;
    public async Task<IEnumerable<SummaryReportDto>> GetSummaryReportAsync(
     int bankId,
     DateOnly fromDate,
     DateOnly toDate,
     int severity,
     bool agentType,
     CancellationToken cancellationToken = default)
    {
        try
        {
            // Step 1: Use IQueryable and apply all filters at database level
            var baseQuery = _cRDBContext.ChequeBookRequisitions
                .AsNoTracking()
                .Where(r => r.IsAgent == agentType &&
                           r.BankId == bankId &&
                           r.RequestDate >= fromDate &&
                           r.RequestDate <= toDate);

            // Step 2: Single optimized query with proper indexing hints
            var query = from requisition in baseQuery
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
                        select new
                        {
                            requisition.Id,
                            requisition.BankId,
                            requisition.ChequeType,
                            requisition.Leaves,
                            requisition.BookQty,
                            requisition.RequestDate,
                            requisition.IsAgent,
                            challan.ChallanNumber,
                            challan.ChallanDate,
                            HomeBranchName = homeBranch.BranchName,
                            DeliveryBranchName = deliveryBranch.BranchName,
                            deliveryBranchAddress = deliveryBranch.BranchAddress,
                            deliveryBranchPhone = deliveryBranch.BranchPhone,
                            courier.CourierName
                        };

            // Step 3: Execute query and do grouping in memory for better performance
            var rawData = await query.ToListAsync(cancellationToken);

            // Step 4: Group in memory (much faster than database grouping for complex calculations)
            var groupedData = rawData
                .GroupBy(x => new
                {
                    x.ChallanNumber,
                    x.ChallanDate,
                    x.HomeBranchName,
                    x.DeliveryBranchName,
                    x.IsAgent
                })
                .OrderBy(g => g.Key.ChallanNumber)
                .Select(g => new SummaryReportDto
                {
                    HomeBranch = g.Key.HomeBranchName,
                    BankId = g.First().BankId,
                    DeliveryBranch = g.Key.DeliveryBranchName,
                    ChallanNo = g.Key.ChallanNumber,
                    ChallanDate = (DateOnly)g.Key.ChallanDate,
                    IsAgent = g.Key.IsAgent??false,
                    CourierName = g.First().CourierName,
                    RequestDate = g.First().RequestDate,
                    BranchAddress = g.First().deliveryBranchAddress,
                    BranchPhone = g.First().deliveryBranchPhone,

                    // Use optimized calculation methods
                    Sb5 = CalculateQtyFast(g, "Savings", 5),
                    Sb10 = CalculateQtyFast(g, "Savings", 10),
                    Sb20 = CalculateQtyFast(g, "Savings", 20),
                    Sb25 = CalculateQtyFast(g, "Savings", 25),
                    Sb50 = CalculateQtyFast(g, "Savings", 50),

                    Cd5 = CalculateQtyFast(g, "Current", 5),
                    Cd10 = CalculateQtyFast(g, "Current", 10),
                    Cd20 = CalculateQtyFast(g, "Current", 20),
                    Cd25 = CalculateQtyFast(g, "Current", 25),
                    Cd50 = CalculateQtyFast(g, "Current", 50),
                    Cd100 = CalculateQtyFast(g, "Current", 100),

                    Po50 = CalculateQtyFast(g, "Payment Order", 50),
                    Po100 = CalculateQtyFast(g, "Payment Order", 100),

                    Ca50 = CalculateQtyFast(g, "Cash Credit", 50),
                    Ca100 = CalculateQtyFast(g, "Cash Credit", 100),

                    Sba10 = CalculateQtyFast(g, "SBA", 10),
                    Msd10 = CalculateQtyFast(g, "MSD", 10),
                    Msd50 = CalculateQtyFast(g, "MSD", 50),

                    Cda25 = CalculateQtyFast(g, "CDA", 25),
                    Acd25 = CalculateQtyFast(g, "ACD", 25),
                    Acd50 = CalculateQtyFast(g, "ACD", 50),
                    Acd100 = CalculateQtyFast(g, "ACD", 100),
                    Awcd25 = CalculateQtyFast(g, "AWCD", 25),
                    Sna25 = CalculateQtyFast(g, "SNA", 25),
                    Snd25 = CalculateQtyFast(g, "SND", 25),
                    Snd50 = CalculateQtyFast(g, "SND", 50),
                    Snd100 = CalculateQtyFast(g, "SND", 100),
                    Msnd25 = CalculateQtyFast(g, "MSND", 25),

                    Poa50 = CalculateQtyFast(g, "POA", 50),
                    Poi50 = CalculateQtyFast(g, "POI", 50),

                    Fdr50 = CalculateQtyFast(g, "FDR", 50),
                    Fdr100 = CalculateQtyFast(g, "FDR", 100),
                    Mtdr25 = CalculateQtyFast(g, "MTDR", 25),
                    Mtdr50 = CalculateQtyFast(g, "MTDR", 50),

                    Total = g.Sum(x => x.BookQty)
                });

            return groupedData.ToList();
        }
        catch (DbException ex)
        {
            throw new Exception($"Database error while generating summary report: {ex.InnerException?.Message ?? ex.Message}", ex);
        }
    }


    // Alternative: Even faster with precomputed dictionary
    //private static int CalculateQtyFaster(ILookup<(string ChequeType, int Leaves), dynamic> lookup,
    //    string chequeType, int leaves)
    //{
    //    return lookup[(chequeType, leaves)].Sum(x => x.BookQty);
    //}
    public async Task<IEnumerable<SummaryReportDto>> GetCourierSummaryReportAsync(
     int bankId,
     DateOnly fromDate,
     DateOnly toDate,
     int severity,
     bool agentType,
     string? courierCode,
     CancellationToken cancellationToken = default)
    {
        try
        {
            // Step 1: Base query (only filters)
            var baseQuery = _cRDBContext.ChequeBookRequisitions
                .AsNoTracking()
                .Where(r =>
                    r.IsAgent == agentType &&
                    r.Serverity == severity &&
                    r.BankId == bankId);


            if (!string.IsNullOrEmpty(courierCode))
            {
                baseQuery = baseQuery.Where(r => r.CourierCode == courierCode);
            }

            // Step 2: Join all required tables
            var query = from requisition in baseQuery
                        join tracking in _cRDBContext.ChallanDetails.AsNoTracking()
                            on requisition.Id equals tracking.RequisitionItemId
                        join challan in _cRDBContext.Challans.AsNoTracking()
                            on tracking.ChallanId equals challan.Id
                        join deliveryBranch in _cRDBContext.Branches.AsNoTracking()
                            on requisition.ReceivingBranchId equals deliveryBranch.Id
                        join courier in _cRDBContext.Couriers.AsNoTracking()
                            on requisition.CourierCode equals courier.CourierCode
                        where challan.ChallanDate >= fromDate &&
                      challan.ChallanDate <= toDate
                        select new
                        {
                            requisition.Id,
                            requisition.BankId,
                            requisition.ChequeType,
                            requisition.Leaves,
                            requisition.BookQty,
                            requisition.RequestDate,
                            requisition.IsAgent,
                            challan.ChallanNumber,
                            challan.ChallanDate,
                            DeliveryBranchName = deliveryBranch.BranchName,
                            deliveryBranchAddress=deliveryBranch.BranchAddress,
                            deliveryBranchPhone=deliveryBranch.BranchPhone,
                            courier.CourierName
                        };

            // Step 3: Execute and load in memory
            var rawData = await query.ToListAsync(cancellationToken);

            // Step 4: Group in memory (faster)
            var groupedData = rawData
                .GroupBy(g => new
                {
                    g.ChallanNumber,
                    g.ChallanDate,
                    g.DeliveryBranchName,
                    g.IsAgent
                })
                .OrderBy(g => g.Key.ChallanNumber)
                .Select(g => new SummaryReportDto
                {
                    DeliveryBranch = g.Key.DeliveryBranchName,
                    BankId=g.First().BankId,
                    ChallanNo = g.Key.ChallanNumber,
                    ChallanDate = (DateOnly)g.Key.ChallanDate,
                    IsAgent = g.Key.IsAgent ?? false,
                    CourierName = g.First().CourierName,
                    RequestDate = g.First().RequestDate,
                    BranchAddress=g.First().deliveryBranchAddress,
                    BranchPhone=g.First().deliveryBranchPhone,

                    Sb5 = CalculateQtyFast(g, "Savings", 5),
                    Sb10 = CalculateQtyFast(g, "Savings", 10),
                    Sb20 = CalculateQtyFast(g, "Savings", 20),
                    Sb25 = CalculateQtyFast(g, "Savings", 25),
                    Sb50 = CalculateQtyFast(g, "Savings", 50),

                    Cd5 = CalculateQtyFast(g, "Current", 5),
                    Cd10 = CalculateQtyFast(g, "Current", 10),
                    Cd20 = CalculateQtyFast(g, "Current", 20),
                    Cd25 = CalculateQtyFast(g, "Current", 25),
                    Cd50 = CalculateQtyFast(g, "Current", 50),
                    Cd100 = CalculateQtyFast(g, "Current", 100),

                    Po50 = CalculateQtyFast(g, "Payment Order", 50),
                    Po100 = CalculateQtyFast(g, "Payment Order", 100),

                    Ca50 = CalculateQtyFast(g, "Cash Credit", 50),
                    Ca100 = CalculateQtyFast(g, "Cash Credit", 100),

                    Sba10 = CalculateQtyFast(g, "SBA", 10),
                    Msd10 = CalculateQtyFast(g, "MSD", 10),
                    Msd50 = CalculateQtyFast(g, "MSD", 50),

                    Cda25 = CalculateQtyFast(g, "CDA", 25),
                    Acd25 = CalculateQtyFast(g, "ACD", 25),
                    Acd50 = CalculateQtyFast(g, "ACD", 50),
                    Acd100 = CalculateQtyFast(g, "ACD", 100),
                    Awcd25 = CalculateQtyFast(g, "AWCD", 25),
                    Sna25 = CalculateQtyFast(g, "SNA", 25),
                    Snd25 = CalculateQtyFast(g, "SND", 25),
                    Snd50 = CalculateQtyFast(g, "SND", 50),
                    Snd100 = CalculateQtyFast(g, "SND", 100),
                    Msnd25 = CalculateQtyFast(g, "MSND", 25),

                    Poa50 = CalculateQtyFast(g, "POA", 50),
                    Poi50 = CalculateQtyFast(g, "POI", 50),

                    Fdr50 = CalculateQtyFast(g, "FDR", 50),
                    Fdr100 = CalculateQtyFast(g, "FDR", 100),
                    Mtdr25 = CalculateQtyFast(g, "MTDR", 25),
                    Mtdr50 = CalculateQtyFast(g, "MTDR", 50),

                    Total = g.Sum(x => x.BookQty)
                });

            return groupedData.ToList();
        }
        catch (DbException ex)
        {
            throw new Exception("Database update error: " + (ex.InnerException?.Message ?? ex.Message), ex);
        }
    }

    public async Task<IEnumerable<DailyProductionReportDto>> GetDailyProductionReportAsync(
     DateTime date,
     CancellationToken cancellationToken = default)
    {
        try
        {
            // Step 1: Filter by date safely (nullable DateTime)
            var baseQuery = _cRDBContext.ChequeBookRequisitions
                .AsNoTracking()
                .Where(r => r.CreatedAt.HasValue && r.CreatedAt.Value.Date == date.Date &&
                !r.IsDeleted);

            // Step 2: Join with Banks table
            var query = from requisition in baseQuery
                        join bank in _cRDBContext.Banks.AsNoTracking()
                            on requisition.BankId equals bank.Id
                        select new
                        {
                            requisition.BankId,
                            requisition.Leaves,
                            requisition.BookQty,
                            requisition.TransactionCode,
                            requisition.ReceivingBranchId,
                            bank.BankName
                        };

            var rawData = await query.ToListAsync(cancellationToken);

            // Step 3: Group by BankId only (one row per bank)
            var groupedData = rawData
                .GroupBy(x => x.BankId)
                .OrderBy(g => g.Key)
                .Select(g => new DailyProductionReportDto
                {
                    BankName = g.First().BankName,

                    // TransactionCode wise aggregation
                    Sb = g.Sum(x => x.TransactionCode == 10 ? (x.Leaves * x.BookQty) : 0),
                    Cd = g.Sum(x => x.TransactionCode == 11 || x.TransactionCode==13 ? (x.Leaves * x.BookQty) : 0),
                    Po = g.Sum(x => x.TransactionCode == 19 ? (x.Leaves * x.BookQty) : 0),
                    A4 = g.Sum(x => x.TransactionCode == 15 ? (x.Leaves * x.BookQty) : 0),
                    MtdrFdr = g.Sum(x => x.TransactionCode == 12 ? (x.Leaves * x.BookQty) : 0),

                    // Totals
                    TotalLeaves = g.Sum(x => x.Leaves * x.BookQty),
                    TotalBooks = g.Sum(x => x.BookQty),

                    // Total distinct branches (nullable safe)
                    TotalBranch = g  // null remove
                        .Select(x => x.ReceivingBranchId)       // int? -> int
                        .Distinct()
                        .Count()
                });

            return groupedData.ToList();
        }
        catch (DbException ex)
        {
            throw new Exception(
                $"Database error while generating summary report: {ex.InnerException?.Message ?? ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<ConsumptionReportDto>> GetConsumptionReportAsync(int BankId, DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken = default)
    {
        try
        {
            // Step 1: Use IQueryable and apply all filters at database level
            var baseQuery = _cRDBContext.ChequeBookRequisitions
                .AsNoTracking()
                .Where(r =>r.BankId == BankId &&
                           r.RequestDate >= fromDate &&
                           r.RequestDate <= toDate && !r.IsDeleted);

            // Step 2: Single optimized query with proper indexing hints
            var query = from requisition in baseQuery
                        select new
                        {
                            requisition.Id,
                            requisition.ChequeType,
                            requisition.Leaves,
                            requisition.BookQty,
                            requisition.RequestDate,
                            requisition.AccFlag
                        };

            // Step 3: Execute query and do grouping in memory for better performance
            var rawData = await query.ToListAsync(cancellationToken);

            // Step 4: Group in memory (much faster than database grouping for complex calculations)
            var groupedData = rawData
                .GroupBy(x => new
                {
                    x.RequestDate,
                })
                .OrderBy(g => g.Key.RequestDate)
                .Select(g => new ConsumptionReportDto
                {
                    RequestDate = g.First().RequestDate,

                    // Use optimized calculation methods
                    Csbcd5Books = CalculateQtyFast(g, "", 5,"General"),
                    Csbcd5Leaves = CalculateLeavesFast(g, "", 5,"General"),
                    Csbcd10Books = CalculateQtyFast(g, "", 10,"General"),
                    Csbcd10Leaves = CalculateLeavesFast(g, "", 10,"General"),
                    Csbcd20Books = CalculateQtyFast(g, "", 20,"General"),
                    Csbcd20Leaves = CalculateLeavesFast(g, "", 20,"General"),
                    Csbcd50Books = CalculateQtyFast(g, "", 50,"General"),
                    Csbcd50Leaves = CalculateLeavesFast(g, "", 50,"General"),
                    Isbcd5Books= CalculateQtyFast(g, "", 5, "Islamic"),
                    Isbcd5Leaves = CalculateLeavesFast(g, "", 5, "Islamic"),
                    Isbcd10Books = CalculateQtyFast(g, "", 10, "Islamic"),
                    Isbcd10Leaves = CalculateLeavesFast(g, "", 10, "Islamic"),
                    Isbcd20Books = CalculateQtyFast(g, "", 20, "Islamic"),
                    Isbcd20Leaves = CalculateLeavesFast(g, "", 20, "Islamic"),
                    Isbcd50Books = CalculateQtyFast(g, "", 50, "Islamic"),
                    Isbcd50Leaves = CalculateLeavesFast(g, "", 50, "Islamic"),
                    Psbcd5Books = CalculateQtyFast(g, "", 5, "Prority"),
                    Psbcd5Leaves = CalculateLeavesFast(g, "", 5, "Prority"),
                    Psbcd10Books = CalculateQtyFast(g, "", 10, "Prority"),
                    Psbcd10Leaves = CalculateLeavesFast(g, "", 10, "Prority"),
                    Psbcd20Books = CalculateQtyFast(g, "", 20, "Prority"),
                    Psbcd20Leaves = CalculateLeavesFast(g, "", 20, "Prority"),
                    Psbcd50Books = CalculateQtyFast(g, "", 50, "Prority"),
                    Psbcd50Leaves = CalculateLeavesFast(g, "", 50, "Prority"),
                    Sb10Books = CalculateQtyFast(g, "Savings", 10),
                    Sb10Leaves = CalculateLeavesFast(g, "Savings", 10),
                    Sb20Books = CalculateQtyFast(g, "Savings", 20),
                    Sb20Leaves = CalculateLeavesFast(g, "Savings", 20),
                    Sb25Books = CalculateQtyFast(g, "Savings", 25),
                    Sb25Leaves = CalculateLeavesFast(g, "Savings", 25),

                   
                    Cd10Books = CalculateQtyFast(g, "Current", 10),
                    Cd10Leaves = CalculateLeavesFast(g, "Current", 10),
                    Cd25Books = CalculateQtyFast(g, "Current", 25),
                    Cd25Leaves = CalculateLeavesFast(g, "Current", 25),
                    Cd50Books = CalculateQtyFast(g, "Current", 50),
                    Cd50Leaves = CalculateLeavesFast(g, "Current", 50),
                    Cd100Books = CalculateQtyFast(g, "Current", 100),
                    Cd100Leaves = CalculateLeavesFast(g, "Current", 100),

                    Po50Books = CalculateQtyFast(g, "Payment Order", 50),
                    Po50Leaves = CalculateLeavesFast(g, "Payment Order", 50),
                    Po100Books = CalculateQtyFast(g, "Payment Order", 100),
                    Po100Leaves = CalculateLeavesFast(g, "Payment Order", 100),

                    Cc50Books = CalculateQtyFast(g, "Cash Credit", 50),
                    Cc50Leaves = CalculateLeavesFast(g, "Cash Credit", 50),
                    Cc100Books = CalculateQtyFast(g, "Cash Credit", 100),
                    Cc100Leaves = CalculateLeavesFast(g, "Cash Credit", 100),

                    Sba10Books = CalculateQtyFast(g, "SBA", 10),
                    Sba10Leaves = CalculateLeavesFast(g, "SBA", 10),
                    Msd10Books = CalculateQtyFast(g, "MSD", 10),
                    Msd10Leaves = CalculateLeavesFast(g, "MSD", 10),
                    Msd50Books = CalculateQtyFast(g, "MSD", 50),
                    Msd50Leaves = CalculateLeavesFast(g, "MSD", 50),


                    Cda25Books = CalculateQtyFast(g, "CDA", 25),
                    Cda25Leaves = CalculateLeavesFast(g, "CDA", 25),
                    Acd25Books = CalculateQtyFast(g, "ACD", 25),
                    Acd25Leaves = CalculateLeavesFast(g, "ACD", 25),
                    Acd50Books = CalculateQtyFast(g, "ACD", 50),
                    Acd50Leaves = CalculateLeavesFast(g, "ACD", 50),
                    Acd100Books = CalculateQtyFast(g, "ACD", 100),
                    Acd100Leaves = CalculateLeavesFast(g, "ACD", 100),
                    Awcd25Books = CalculateQtyFast(g, "AWCD", 25),
                    Awcd25Leaves = CalculateLeavesFast(g, "AWCD", 25),
                    Snd25Books = CalculateQtyFast(g, "SND", 25),
                    Snd25Leaves = CalculateLeavesFast(g, "SND", 25),
                    Snd50Books = CalculateQtyFast(g, "SND", 50),
                    Snd50Leaves = CalculateLeavesFast(g, "SND", 50),
                    Snd100Books = CalculateQtyFast(g, "SND", 100),
                    Snd100Leaves = CalculateLeavesFast(g, "SND", 100),
                    Sna25Books = CalculateQtyFast(g, "SNA", 25),
                    Sna25Leaves = CalculateLeavesFast(g, "SNA", 25),
                    Msnd25Books = CalculateQtyFast(g, "MSND", 25),
                    Msnd25Leaves = CalculateLeavesFast(g, "MSND", 25),

                    Poa50Books = CalculateQtyFast(g, "POA", 50),
                    Poa50Leaves = CalculateLeavesFast(g, "POA", 50),
                    Poi50Books = CalculateQtyFast(g, "POI", 50),
                    Poi50Leaves = CalculateLeavesFast(g, "POI", 50),

                    Fdr50Books = CalculateQtyFast(g, "FDR", 50),
                    Fdr50Leaves = CalculateLeavesFast(g, "FDR", 50),
                    Fdr100Books = CalculateQtyFast(g, "FDR", 100),
                    Fdr100Leaves = CalculateLeavesFast(g, "FDR", 100),
                    Mtdr25Books = CalculateQtyFast(g, "MTDR", 25),
                    Mtdr25Leaves = CalculateLeavesFast(g, "MTDR", 25),
                    Mtdr50Books = CalculateQtyFast(g, "MTDR", 50),
                    Mtdr50Leaves = CalculateLeavesFast(g, "MTDR", 50),

                    TotalBooks = g.Sum(x => x.BookQty),
                    TotalLeaves = g.Sum(x => x.BookQty * x.Leaves)
                });

            return groupedData.ToList();
        }
        catch (DbException ex)
        {
            throw new Exception($"Database error while generating summary report: {ex.InnerException?.Message ?? ex.Message}", ex);
        }
    }

    // Optimized calculation method using Where instead of ternary in Sum
    private static int CalculateQtyFast(IEnumerable<dynamic> group, string? chequeType, int leaves, string? accFlag=null)
    {
        if (!string.IsNullOrEmpty(accFlag))
        {
            return group
                .Where(x => x.Leaves == leaves && x.AccFlag == accFlag)
                .Sum(x => x.BookQty);
        }
        return group
            .Where(x => x.ChequeType == chequeType && x.Leaves == leaves)
            .Sum(x => x.BookQty);
    }

    // Optimized calculation method using Where instead of ternary in Sum
    private static int? CalculateLeavesFast(IGrouping<dynamic, dynamic> group, string? chequeType, int leaves, string? accFlag = null)
    {
        if (!string.IsNullOrEmpty(accFlag))
        {
             return group
                .Where(x => x.Leaves == leaves && x.AccFlag == accFlag)
                .Sum(x => (int?)x.BookQty * (int?)x.Leaves);
        }
        return group
            .Where(x => x.ChequeType == chequeType && x.Leaves == leaves)
            .Sum(x => (int?)x.BookQty * (int?)x.Leaves);
    }

}
