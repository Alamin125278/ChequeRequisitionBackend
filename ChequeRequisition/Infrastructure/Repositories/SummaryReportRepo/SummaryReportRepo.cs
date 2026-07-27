using ChequeRequisiontService.Core.Dto.SummaryReport;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.DbContexts;
using ChequeRequisiontService.Endpoints.SummaryReport.ConsumptionReport;
using ChequeRequisiontService.Models.CRDB;
using DocumentFormat.OpenXml.Spreadsheet;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace ChequeRequisiontService.Infrastructure.Repositories.SummaryReportRepo;

public class SummaryReportRepo(CRDBContext cRDBContext) : ISummaryReport

{
    private CRDBContext _cRDBContext = cRDBContext;
    public async Task<IEnumerable<BranchWiseBillDto>> GetSummaryReportAsync(
     int bankId,
     DateOnly fromDate,
     DateOnly toDate,
     int severity,
     bool? agentType,
     CancellationToken cancellationToken = default)
    {
        try
        {
            // Step 1: Use IQueryable and apply all filters at database level
            var baseQuery = _cRDBContext.ChequeBookRequisitions
                .AsNoTracking()
                .Where(r =>
                           r.BankId == bankId &&
                           r.RequestDate >= fromDate &&
                           r.RequestDate <= toDate);
            if (agentType.HasValue)
            {
                baseQuery = baseQuery.Where(r => r.IsAgent == agentType.Value);
            }

            // Step 2: Single optimized query with proper indexing hints
            var query = from requisition in baseQuery
                        join tracking in _cRDBContext.ChallanDetails.AsNoTracking()
                            on requisition.Id equals tracking.RequisitionItemId
                        join deliveryBranch in _cRDBContext.Branches.AsNoTracking()
                            on requisition.ReceivingBranchId equals deliveryBranch.Id
                        select new
                        {
                            requisition.Id,
                            requisition.BankId,
                            requisition.ChequeType,
                            requisition.Leaves,
                            requisition.BookQty,
                            requisition.RequestDate,
                            requisition.IsAgent,
                            requisition.AccFlag,
                            requisition.DistId,
                            DeliveryBranchName = deliveryBranch.BranchName,
                        };

            // Step 3: Execute query and do grouping in memory for better performance
            var rawData = await query.ToListAsync(cancellationToken);

            // Step 4: Group in memory (much faster than database grouping for complex calculations)
            var groupedData = rawData
                .GroupBy(x => new
                {
                    x.DeliveryBranchName,
                    x.IsAgent,
                    x.DistId
                })
                .Select(g => new BranchWiseBillDto
                {
                    BankId = g.First().BankId,
                    DeliveryBranch = g.Key.DeliveryBranchName,
                    IsAgent = g.Key.IsAgent??false,
                    DistId = g.Key.DistId,

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
                    Msa10 = CalculateQtyFast(g, "MSA", 10),
                    Msa20 = CalculateQtyFast(g, "MSA", 20),
                    Msd50 = CalculateQtyFast(g, "MSD", 50),

                    Cda25 = CalculateQtyFast(g, "CDA", 25),
                    Acd25 = CalculateQtyFast(g, "ACD", 25),
                    Acd50 = CalculateQtyFast(g, "ACD", 50),
                    Acd100 = CalculateQtyFast(g, "ACD", 100),
                    Awcd25 = CalculateQtyFast(g, "AWCD", 25),
                    Awca20 = CalculateQtyFast(g, "AWCA", 20),
                    Awca50 = CalculateQtyFast(g, "AWCA", 50),
                    Awca100 = CalculateQtyFast(g, "AWCA", 100),
                    Msna50 = CalculateQtyFast(g, "MSNA", 50),
                    Msna100 = CalculateQtyFast(g, "MSNA", 100),
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
                    Conv5 = CalculateQtyFast(g, null, 5, "CONV"),
                    Conv10 = CalculateQtyFast(g, null, 10, "CONV"),
                    Conv20 = CalculateQtyFast(g, null, 20, "CONV"),
                    Conv50 = CalculateQtyFast(g, null, 50, "CONV"),
                    Islm5 = CalculateQtyFast(g, null, 5, "Islamic"),
                    Islm10 = CalculateQtyFast(g, null, 10, "Islamic"),
                    Islm20 = CalculateQtyFast(g, null, 20, "Islamic"),
                    Islm50 = CalculateQtyFast(g, null, 50, "Islamic"),
                    Prio10 = CalculateQtyFast(g, null, 10, "Priority"),
                    Prio20 = CalculateQtyFast(g, null, 20, "Priority"),
                    Prio50 = CalculateQtyFast(g, null, 50, "Priority"),

                    Total = g.Sum(x => x.BookQty),
                    TotalLeaves = g.Sum(x => x.BookQty * x.Leaves),
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
                            courier.CourierName,
                            requisition.AccFlag
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
                    Msa10 = CalculateQtyFast(g, "MSA", 10),
                    Msa20 = CalculateQtyFast(g, "MSA", 20),
                    Msd50 = CalculateQtyFast(g, "MSD", 50),

                    Cda25 = CalculateQtyFast(g, "CDA", 25),
                    Acd25 = CalculateQtyFast(g, "ACD", 25),
                    Acd50 = CalculateQtyFast(g, "ACD", 50),
                    Acd100 = CalculateQtyFast(g, "ACD", 100),
                    Awcd25 = CalculateQtyFast(g, "AWCD", 25),
                    Awca20 = CalculateQtyFast(g, "AWCA", 20),
                    Awca50 = CalculateQtyFast(g, "AWCA", 50),
                    Awca100 = CalculateQtyFast(g, "AWCA", 100),
                    Msna50 = CalculateQtyFast(g, "MSNA", 50),
                    Msna100 = CalculateQtyFast(g, "MSNA", 100),
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
                    Conv5 = CalculateQtyFast(g, null, 5, "CONV"),
                    Conv10 = CalculateQtyFast(g, null, 10, "CONV"),
                    Conv20 = CalculateQtyFast(g, null, 20, "CONV"),
                    Conv50 = CalculateQtyFast(g, null, 50, "CONV"),
                    Islm5 = CalculateQtyFast(g, null, 5,"Islamic"),
                    Islm10 = CalculateQtyFast(g, null, 10, "Islamic"),
                    Islm20 = CalculateQtyFast(g, null, 20, "Islamic"),
                    Islm50 = CalculateQtyFast(g, null, 50, "Islamic"),
                    Prio10 = CalculateQtyFast(g, null, 10, "Priority"),
                    Prio20 = CalculateQtyFast(g, null, 20, "Priority"),
                    Prio50 = CalculateQtyFast(g, null, 50, "Priority"),

                    Total = g.Sum(x => x.BookQty)
                });

            return groupedData.ToList();
        }
        catch (DbException ex)
        {
            throw new Exception("Database update error: " + (ex.InnerException?.Message ?? ex.Message), ex);
        }
    }

    public async Task<IEnumerable<AgentSummaryReportDto>> GetAgentSummaryReportAsync(int BankId, DateOnly RequestDate, bool? AgentType, string? Courier, CancellationToken cancellationToken = default)
    {
        try
        {
            var baseQuery = _cRDBContext.ChequeBookRequisitions
                .AsNoTracking()
                .Where(r => r.RequestDate == RequestDate &&
                            r.BankId == BankId);

            if (!string.IsNullOrEmpty(Courier))
            {
                baseQuery = baseQuery.Where(r => r.CourierCode == Courier);
            }

            if (AgentType.HasValue)
            {
                baseQuery = baseQuery.Where(r => r.IsAgent == AgentType.Value);
            }

            var rawData = await (
                from requisition in baseQuery
                join deliveryBranch in _cRDBContext.Branches.AsNoTracking()
                    on requisition.ReceivingBranchId equals deliveryBranch.Id
                join courier in _cRDBContext.Couriers.AsNoTracking()
                        on requisition.CourierCode equals courier.CourierCode
                select new
                {
                    requisition.BankId,
                    requisition.ChequeType,
                    requisition.Leaves,
                    requisition.BookQty,
                    requisition.RequestDate,
                    requisition.DistId,
                    requisition.ReceivingBranchId,
                    DeliveryBranchName = deliveryBranch.BranchName,
                    courier.CourierName
                })
                .ToListAsync(cancellationToken);

            if (AgentType == true)
            {
                return rawData
                    .GroupBy(x => new { x.DistId, x.ReceivingBranchId })
                    .Select(g =>
                    {
                        var first = g.First();

                        return new AgentSummaryReportDto
                        {
                            DeliveryBranch = first.DeliveryBranchName,
                            CourierName = g.First().CourierName,
                            BankId = first.BankId,
                            RequestDate = first.RequestDate,
                            DistId = first.DistId,


                            Msa10 = CalculateQtyFast(g, "MSA", 10),
                            Msa20 = CalculateQtyFast(g, "MSA", 20),
                            Awca20 = CalculateQtyFast(g, "AWCA", 20),
                            Awca50 = CalculateQtyFast(g, "AWCA", 50),
                            Awca100 = CalculateQtyFast(g, "AWCA", 100),
                            Po50 = CalculateQtyFast(g, "PO", 50),
                            Total = g.Sum(x => x.BookQty)
                        };
                    })
                    .ToList();
            }

            return rawData
                .GroupBy(x => x.ReceivingBranchId)
                .Select(g =>
                {
                    var first = g.First();

                    return new AgentSummaryReportDto
                    {
                        DeliveryBranch = first.DeliveryBranchName,
                        CourierName = g.First().CourierName,
                        BankId = first.BankId,
                        RequestDate = first.RequestDate,
                        DistId = string.Empty,

                        Msa10 = CalculateQtyFast(g, "MSA", 10),
                        Msa20 = CalculateQtyFast(g, "MSA", 20),
                        Awca20 = CalculateQtyFast(g, "AWCA", 20),
                        Awca50 = CalculateQtyFast(g, "AWCA", 50),
                        Awca100 = CalculateQtyFast(g, "AWCA", 100),
                        Po50 = CalculateQtyFast(g, "PO", 50),
                        Total = g.Sum(x => x.BookQty)
                    };
                })
                .ToList();
        }
        catch (DbException ex)
        {
            throw new Exception(
                "Database update error: " + (ex.InnerException?.Message ?? ex.Message),
                ex);
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
    public async Task<IEnumerable<AgentReceiptDto>> GetAgentReceiptReportAsync(int BankId, DateOnly RequestDate, CancellationToken cancellationToken = default)
    {
        try
        {
            // Step 1: Filter by date safely (nullable DateTime)
            var baseQuery = _cRDBContext.ChequeBookRequisitions
                .AsNoTracking()
                .Where(r=>r.BankId==BankId && r.RequestDate==RequestDate&& r.DistId !="");

            // Step 2: Join with Banks table
            var query = from r in baseQuery
                        join bank in _cRDBContext.Banks.AsNoTracking()
                            on r.BankId equals bank.Id
                        join rbrc in _cRDBContext.Branches on r.ReceivingBranchId equals rbrc.Id into rrbr
                        from rbrc in rrbr.DefaultIfEmpty()
                        join d in _cRDBContext.ChallanDetails on r.Id equals d.RequisitionItemId into rd
                        from d in rd.DefaultIfEmpty()
                        join c in _cRDBContext.Challans on d.ChallanId equals c.Id into rc
                        from c in rc.DefaultIfEmpty()
                        select new
                        {
                            r.Id,
                            r.BankId,
                            r.AccountName,
                            r.AccountNo,
                            r.StartNo,
                            r.EndNo,
                            r.RequestDate,
                            r.ChequeType,
                            r.Leaves,
                            r.BookQty,
                            r.DistId,
                            r.ReceivingBranchId,
                            DeliveryBranch = rbrc.BranchName,
                            ChallanNo = c.ChallanNumber
                        };

            var rawData = await query.ToListAsync(cancellationToken);

            // Step 4: Group in memory (much faster than database grouping for complex calculations)
            var groupedData = rawData
                .GroupBy(x => new
                {
                    x.ReceivingBranchId,x.DistId
                })
                .Select(g => new AgentReceiptDto
                {
                    DeliveryBranch = g.First().DeliveryBranch,
                    DistId = g.First().DistId,
                    ChallanNo = g.First().ChallanNo,
                    TotalBookQty = g.Sum(x => x.BookQty),
                    Items = g.Select(x => new ChequeBookItemDto
                    {
                        AccountNo = x.AccountNo,
                        AccountName = x.AccountName,
                        StartNo = x.StartNo,
                        EndNo = x.EndNo,
                        AccType = x.ChequeType,
                        BookQty = x.BookQty,
                        Leaves = x.Leaves,
                    }).ToList()

                });

            return groupedData.ToList();
        }catch(DbException ex)
        {
            throw new Exception($"Database error while generating summary report: {ex.InnerException?.Message ?? ex.Message}", ex);
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
                    Csbcd5Books = CalculateQtyFast(g, "", 5,"CONV"),
                    Csbcd5Leaves = CalculateLeavesFast(g, "", 5,"CONV"),
                    Csbcd10Books = CalculateQtyFast(g, "", 10,"CONV"),
                    Csbcd10Leaves = CalculateLeavesFast(g, "", 10,"CONV"),
                    Csbcd20Books = CalculateQtyFast(g, "", 20,"CONV"),
                    Csbcd20Leaves = CalculateLeavesFast(g, "", 20,"CONV"),
                    Csbcd50Books = CalculateQtyFast(g, "", 50,"CONV"),
                    Csbcd50Leaves = CalculateLeavesFast(g, "", 50,"CONV"),
                    Isbcd5Books= CalculateQtyFast(g, "", 5, "Islamic"),
                    Isbcd5Leaves = CalculateLeavesFast(g, "", 5, "Islamic"),
                    Isbcd10Books = CalculateQtyFast(g, "", 10, "Islamic"),
                    Isbcd10Leaves = CalculateLeavesFast(g, "", 10, "Islamic"),
                    Isbcd20Books = CalculateQtyFast(g, "", 20, "Islamic"),
                    Isbcd20Leaves = CalculateLeavesFast(g, "", 20, "Islamic"),
                    Isbcd50Books = CalculateQtyFast(g, "", 50, "Islamic"),
                    Isbcd50Leaves = CalculateLeavesFast(g, "", 50, "Islamic"),
                    Psbcd5Books = CalculateQtyFast(g, "", 5, "Priority"),
                    Psbcd5Leaves = CalculateLeavesFast(g, "", 5, "Priority"),
                    Psbcd10Books = CalculateQtyFast(g, "", 10, "Priority"),
                    Psbcd10Leaves = CalculateLeavesFast(g, "", 10, "Priority"),
                    Psbcd20Books = CalculateQtyFast(g, "", 20, "Priority"),
                    Psbcd20Leaves = CalculateLeavesFast(g, "", 20, "Priority"),
                    Psbcd50Books = CalculateQtyFast(g, "", 50, "Priority"),
                    Psbcd50Leaves = CalculateLeavesFast(g, "", 50, "Priority"),
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

                    Po50Books = CalculateQtyFast(g, "PO", 50),
                    Po50Leaves = CalculateLeavesFast(g, "PO", 50),
                    Po100Books = CalculateQtyFast(g, "Payment Order", 100),
                    Po100Leaves = CalculateLeavesFast(g, "Payment Order", 100),

                    Cc50Books = CalculateQtyFast(g, "Cash Credit", 50),
                    Cc50Leaves = CalculateLeavesFast(g, "Cash Credit", 50),
                    Cc100Books = CalculateQtyFast(g, "Cash Credit", 100),
                    Cc100Leaves = CalculateLeavesFast(g, "Cash Credit", 100),

                    Sba10Books = CalculateQtyFast(g, "SBA", 10),
                    Sba10Leaves = CalculateLeavesFast(g, "SBA", 10),
                    Msa10Books = CalculateQtyFast(g, "MSA", 10),
                    Msa10Leaves = CalculateLeavesFast(g, "MSA", 10),
                    Msa20Books = CalculateQtyFast(g, "MSA", 20),
                    Msa20Leaves = CalculateLeavesFast(g, "MSA", 20),
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
                    Awca20Books = CalculateQtyFast(g, "AWCA", 20),
                    Awca20Leaves = CalculateLeavesFast(g, "AWCA", 20),
                    Awca50Books = CalculateQtyFast(g, "AWCA", 50),
                    Awca50Leaves = CalculateLeavesFast(g, "AWCA", 50), 
                    Awca100Books = CalculateQtyFast(g, "AWCA", 100),
                    Awca100Leaves = CalculateLeavesFast(g, "AWCA", 100), 
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
        else
        {
            return group
                .Where(x => x.ChequeType == chequeType && x.Leaves == leaves)
                .Sum(x => x.BookQty);
        }
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
