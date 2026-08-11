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
                    Consb10 = CalculateQtyFast(g, "Savings", 10, "CONV"),
                    Consb20 = CalculateQtyFast(g, "Savings", 20, "CONV"),
                    Consb25 = CalculateQtyFast(g, "Savings", 25, "CONV"),
                    Islmsb10 = CalculateQtyFast(g, "Savings", 10, "Islamic"),
                    Islmsb20 = CalculateQtyFast(g, "Savings", 20, "Islamic"),
                    Islmsb25 = CalculateQtyFast(g, "Savings", 25, "Islamic"),

                    Concd10 = CalculateQtyFast(g, "Current", 10, "CONV"),
                    Concd20 = CalculateQtyFast(g, "Current", 20, "CONV"),
                    Concd25 = CalculateQtyFast(g, "Current", 25, "CONV"),
                    Concd50 = CalculateQtyFast(g, "Current", 50, "CONV"),
                    Islmcd25 = CalculateQtyFast(g, "Current", 25, "Islamic"),
                    Islmcd50 = CalculateQtyFast(g, "Current", 50, "Islamic"),

                    Conpo50 = CalculateQtyFast(g, "Payment Order", 50, "CONV"),
                    Conpo100 = CalculateQtyFast(g, "Payment Order", 100, "CONV"),

                    Conca50 = CalculateQtyFast(g, "Cash Credit", 50, "CONV"),
                    Conca100 = CalculateQtyFast(g, "Cash Credit", 100, "CONV"),

                    Consba10 = CalculateQtyFast(g, "SBA", 10, "CONV"),
                    Conmsd10 = CalculateQtyFast(g, "MSD", 10, "CONV"),
                    Conmsd50 = CalculateQtyFast(g, "MSD", 50, "CONV"),
                    Conmsa10 = CalculateQtyFast(g, "MSA", 10, "CONV"),
                    Conmsa20 = CalculateQtyFast(g, "MSA", 20, "CONV"),

                    Concda25 = CalculateQtyFast(g, "CDA", 25, "CONV"),
                    Conacd25 = CalculateQtyFast(g, "ACD", 25, "CONV"),
                    Conacd50 = CalculateQtyFast(g, "ACD", 50, "CONV"),
                    Conacd100 = CalculateQtyFast(g, "ACD", 100, "CONV"),
                    Conawcd25 = CalculateQtyFast(g, "AWCD", 25, "CONV"),
                    Conawca20 = CalculateQtyFast(g, "AWCA", 20, "CONV"),
                    Conawca50 = CalculateQtyFast(g, "AWCA", 50, "CONV"),
                    Conawca100 = CalculateQtyFast(g, "AWCA", 100, "CONV"),

                    Conpoa50 = CalculateQtyFast(g, "POA", 50, "CONV"),
                    Conpoi50 = CalculateQtyFast(g, "POI", 50, "CONV"),

                    Confdr50 = CalculateQtyFast(g, "FDR", 50, "CONV"),
                    Confdr100 = CalculateQtyFast(g, "FDR", 100, "CONV"),
                    Conmtdr25 = CalculateQtyFast(g, "MTDR", 25, "CONV"),
                    Conmtdr50 = CalculateQtyFast(g, "MTDR", 50, "CONV"),

                    Conv5 = CalculateQtyFast(g, null, 5, "CONV", bankId),
                    Conv10 = CalculateQtyFast(g, null, 10, "CONV", bankId),
                    Conv20 = CalculateQtyFast(g, null, 20, "CONV", bankId),
                    Conv50 = CalculateQtyFast(g, null, 50, "CONV", bankId),
                    Islm5 = CalculateQtyFast(g, null, 5, "Islamic", bankId),
                    Islm10 = CalculateQtyFast(g, null, 10, "Islamic", bankId),
                    Islm20 = CalculateQtyFast(g, null, 20, "Islamic", bankId),
                    Islm50 = CalculateQtyFast(g, null, 50, "Islamic", bankId),
                    Prio10 = CalculateQtyFast(g, null, 10, "Priority", bankId),
                    Prio20 = CalculateQtyFast(g, null, 20, "Priority", bankId),
                    Prio50 = CalculateQtyFast(g, null, 50, "Priority", bankId),

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

                    //Sb5 = CalculateQtyFast(g, "Savings", 5),
                    //Sb10 = CalculateQtyFast(g, "Savings", 10),
                    //Sb20 = CalculateQtyFast(g, "Savings", 20),
                    //Sb25 = CalculateQtyFast(g, "Savings", 25),
                    //Sb50 = CalculateQtyFast(g, "Savings", 50),
                    Consb10 = CalculateQtyFast(g, "Savings", 10, "CONV"),
                    Consb20 = CalculateQtyFast(g, "Savings", 20, "CONV"),
                    Consb25 = CalculateQtyFast(g, "Savings", 25, "CONV"),
                    Islmsb10 = CalculateQtyFast(g, "Savings", 10, "Islamic"),
                    Islmsb20 = CalculateQtyFast(g, "Savings", 20, "Islamic"),
                    Islmsb25 = CalculateQtyFast(g, "Savings", 25, "Islamic"),

                    //Cd10 = CalculateQtyFast(g, "Current", 10),
                    //Cd20 = CalculateQtyFast(g, "Current", 20),
                    //Cd25 = CalculateQtyFast(g, "Current", 25),
                    //Cd50 = CalculateQtyFast(g, "Current", 50),
                    //Cd100 = CalculateQtyFast(g, "Current", 100),
                    Concd10 = CalculateQtyFast(g, "Current", 10,"CONV"),
                    Concd20 = CalculateQtyFast(g, "Current", 20,"CONV"),
                    Concd25 = CalculateQtyFast(g, "Current", 25,"CONV"),
                    Concd50 = CalculateQtyFast(g, "Current", 50,"CONV"),
                    Islmcd25 = CalculateQtyFast(g, "Current", 25,"Islamic"),
                    Islmcd50 = CalculateQtyFast(g, "Current", 50,"Islamic"),



                    //Po50 = CalculateQtyFast(g, "Payment Order", 50),
                    //Po100 = CalculateQtyFast(g, "Payment Order", 100),
                    Conpo50 = CalculateQtyFast(g, "Payment Order", 50,"CONV"),
                    Conpo100 = CalculateQtyFast(g, "Payment Order", 100,"CONV"),

                    //Ca50 = CalculateQtyFast(g, "Cash Credit", 50),
                    //Ca100 = CalculateQtyFast(g, "Cash Credit", 100),
                    Conca50 = CalculateQtyFast(g, "Cash Credit", 50,"CONV"),
                    Conca100 = CalculateQtyFast(g, "Cash Credit", 100,"CONV"),

                    //Sba10 = CalculateQtyFast(g, "SBA", 10),
                    //Msd10 = CalculateQtyFast(g, "MSD", 10),
                    //Msa10 = CalculateQtyFast(g, "MSA", 10),
                    //Msa20 = CalculateQtyFast(g, "MSA", 20),
                    //Msd50 = CalculateQtyFast(g, "MSD", 50),
                    Consba10 = CalculateQtyFast(g, "SBA", 10, "CONV"),
                    Conmsd10= CalculateQtyFast(g, "MSD", 10, "CONV"),
                    Conmsd50= CalculateQtyFast(g, "MSD", 50, "CONV"),
                    Conmsa10 =CalculateQtyFast(g, "MSA", 10, "CONV"),
                    Conmsa20 =CalculateQtyFast(g, "MSA", 20, "CONV"),

                    //Cda25 = CalculateQtyFast(g, "CDA", 25),
                    //Acd25 = CalculateQtyFast(g, "ACD", 25),
                    //Acd50 = CalculateQtyFast(g, "ACD", 50),
                    //Acd100 = CalculateQtyFast(g, "ACD", 100),
                    //Awcd25 = CalculateQtyFast(g, "AWCD", 25),
                    //Awca20 = CalculateQtyFast(g, "AWCA", 20),
                    //Awca50 = CalculateQtyFast(g, "AWCA", 50),
                    //Awca100 = CalculateQtyFast(g, "AWCA", 100),
                    //Msna50 = CalculateQtyFast(g, "MSNA", 50),
                    //Msna100 = CalculateQtyFast(g, "MSNA", 100),
                    //Sna25 = CalculateQtyFast(g, "SNA", 25),
                    //Snd25 = CalculateQtyFast(g, "SND", 25),
                    //Snd50 = CalculateQtyFast(g, "SND", 50),
                    //Snd100 = CalculateQtyFast(g, "SND", 100),
                    //Msnd25 = CalculateQtyFast(g, "MSND", 25),
                    Concda25 = CalculateQtyFast(g, "CDA", 25 ,"CONV"),
                    Conacd25 = CalculateQtyFast(g, "ACD", 25, "CONV"),
                    Conacd50 = CalculateQtyFast(g, "ACD", 50, "CONV"),
                    Conacd100 = CalculateQtyFast(g, "ACD", 100, "CONV"),
                    Conawcd25 = CalculateQtyFast(g, "AWCD", 25, "CONV"),
                    Conawca20 = CalculateQtyFast(g, "AWCA", 20, "CONV"),
                    Conawca50 = CalculateQtyFast(g, "AWCA", 50, "CONV"),
                    Conawca100 = CalculateQtyFast(g, "AWCA", 100, "CONV"),
                    //Conmsna50 = CalculateQtyFast(g, "MSNA", 50),
                    //Conmsna100 = CalculateQtyFast(g, "MSNA", 100),
                    Consna25 = CalculateQtyFast(g, "SNA", 25, "CONV"),
                    Consnd25 = CalculateQtyFast(g, "SND", 25, "CONV"),
                    Consnd50 = CalculateQtyFast(g, "SND", 50, "CONV"),
                    Consnd100 = CalculateQtyFast(g, "SND", 100, "CONV"),
                    Conmsnd25 = CalculateQtyFast(g, "MSND", 25, "CONV"),

                    //Poa50 = CalculateQtyFast(g, "POA", 50),
                    //Poi50 = CalculateQtyFast(g, "POI", 50),

                    //Fdr50 = CalculateQtyFast(g, "FDR", 50),
                    //Fdr100 = CalculateQtyFast(g, "FDR", 100),
                    //Mtdr25 = CalculateQtyFast(g, "MTDR", 25),
                    //Mtdr50 = CalculateQtyFast(g, "MTDR", 50),

                    Conpoa50 = CalculateQtyFast(g, "POA", 50, "CONV"),
                    Conpoi50 = CalculateQtyFast(g, "POI", 50, "CONV"),

                    Confdr50 = CalculateQtyFast(g, "FDR", 50, "CONV"),
                    Confdr100 = CalculateQtyFast(g, "FDR", 100, "CONV"),
                    Conmtdr25 = CalculateQtyFast(g, "MTDR", 25, "CONV"),
                    Conmtdr50 = CalculateQtyFast(g, "MTDR", 50, "CONV"),

                    Conv5 = CalculateQtyFast(g, null, 5, "CONV", bankId),
                    Conv10 = CalculateQtyFast(g, null, 10, "CONV", bankId),
                    Conv20 = CalculateQtyFast(g, null, 20, "CONV", bankId),
                    Conv50 = CalculateQtyFast(g, null, 50, "CONV", bankId),
                    Islm5 = CalculateQtyFast(g, null, 5,"Islamic", bankId),
                    Islm10 = CalculateQtyFast(g, null, 10, "Islamic", bankId),
                    Islm20 = CalculateQtyFast(g, null, 20, "Islamic", bankId),
                    Islm50 = CalculateQtyFast(g, null, 50, "Islamic", bankId),
                    Prio10 = CalculateQtyFast(g, null, 10, "Priority", bankId),
                    Prio20 = CalculateQtyFast(g, null, 20, "Priority", bankId),
                    Prio50 = CalculateQtyFast(g, null, 50, "Priority", bankId),

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


                            Msa10 = CalculateQtyFastIBBL(g, "MSA", 10),
                            Msa20 = CalculateQtyFastIBBL(g, "MSA", 20),
                            Awca20 = CalculateQtyFastIBBL(g, "AWCA", 20),
                            Awca50 = CalculateQtyFastIBBL(g, "AWCA", 50),
                            Awca100 = CalculateQtyFastIBBL(g, "AWCA", 100),
                            Po50 = CalculateQtyFastIBBL(g, "PO", 50),
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

                        Msa10 = CalculateQtyFastIBBL(g, "MSA", 10),
                        Msa20 = CalculateQtyFastIBBL(g, "MSA", 20),
                        Awca20 = CalculateQtyFastIBBL(g, "AWCA", 20),
                        Awca50 = CalculateQtyFastIBBL(g, "AWCA", 50),
                        Awca100 = CalculateQtyFastIBBL(g, "AWCA", 100),
                        Po50 = CalculateQtyFastIBBL(g, "PO", 50),
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
                    Csbcd5Books = CalculateQtyFast(g, "", 5,"CONV", BankId),
                    Csbcd5Leaves = CalculateLeavesFast(g, "", 5,"CONV", BankId),
                    Csbcd10Books = CalculateQtyFast(g, "", 10,"CONV", BankId),
                    Csbcd10Leaves = CalculateLeavesFast(g, "", 10,"CONV", BankId),
                    Csbcd20Books = CalculateQtyFast(g, "", 20,"CONV", BankId),
                    Csbcd20Leaves = CalculateLeavesFast(g, "", 20,"CONV", BankId),
                    Csbcd50Books = CalculateQtyFast(g, "", 50,"CONV", BankId),
                    Csbcd50Leaves = CalculateLeavesFast(g, "", 50,"CONV", BankId),
                    Isbcd5Books= CalculateQtyFast(g, "", 5, "Islamic", BankId),
                    Isbcd5Leaves = CalculateLeavesFast(g, "", 5, "Islamic", BankId),
                    Isbcd10Books = CalculateQtyFast(g, "", 10, "Islamic", BankId),
                    Isbcd10Leaves = CalculateLeavesFast(g, "", 10, "Islamic", BankId),
                    Isbcd20Books = CalculateQtyFast(g, "", 20, "Islamic", BankId),
                    Isbcd20Leaves = CalculateLeavesFast(g, "", 20, "Islamic", BankId),
                    Isbcd50Books = CalculateQtyFast(g, "", 50, "Islamic", BankId),
                    Isbcd50Leaves = CalculateLeavesFast(g, "", 50, "Islamic", BankId),
                    Psbcd10Books = CalculateQtyFast(g, "", 10, "Priority", BankId),
                    Psbcd10Leaves = CalculateLeavesFast(g, "", 10, "Priority", BankId),
                    Psbcd20Books = CalculateQtyFast(g, "", 20, "Priority", BankId),
                    Psbcd20Leaves = CalculateLeavesFast(g, "", 20, "Priority", BankId),
                    Psbcd50Books = CalculateQtyFast(g, "", 50, "Priority", BankId),
                    Psbcd50Leaves = CalculateLeavesFast(g, "", 50, "Priority", BankId),
                    ConSb10Books = CalculateQtyFast(g, "Savings", 10, "CONV"),
                    ConSb10Leaves = CalculateLeavesFast(g, "Savings", 10, "CONV"),
                    ConSb20Books = CalculateQtyFast(g, "Savings", 20, "CONV"),
                    ConSb20Leaves = CalculateLeavesFast(g, "Savings", 20, "CONV"),
                    ConSb25Books = CalculateQtyFast(g, "Savings", 25, "CONV"),
                    ConSb25Leaves = CalculateLeavesFast(g, "Savings", 25, "CONV"),
                    IslmSb10Books = CalculateQtyFast(g, "Savings", 10, "Islamic"),
                    IslmSb10Leaves = CalculateLeavesFast(g, "Savings", 10, "Islamic"),
                    IslmSb20Books = CalculateQtyFast(g, "Savings", 20, "Islamic"),
                    IslmSb20Leaves = CalculateLeavesFast(g, "Savings", 20, "Islamic"),
                    IslmSb25Books = CalculateQtyFast(g, "Savings", 25, "Islamic"),
                    IslmSb25Leaves = CalculateLeavesFast(g, "Savings", 25, "Islamic"),


                    ConCd10Books = CalculateQtyFast(g, "Current", 10, "CONV"),
                    ConCd10Leaves = CalculateLeavesFast(g, "Current", 10, "CONV"),
                    ConCd25Books = CalculateQtyFast(g, "Current", 25, "CONV"),
                    ConCd25Leaves = CalculateLeavesFast(g, "Current", 25, "CONV"),
                    IslmCd25Books = CalculateQtyFast(g, "Current", 25, "Islamic"),
                    IslmCd25Leaves = CalculateLeavesFast(g, "Current", 25, "Islamic"),
                    ConCd50Books = CalculateQtyFast(g, "Current", 50, "CONV"),
                    ConCd50Leaves = CalculateLeavesFast(g, "Current", 50, "CONV"),
                    IslmCd50Books = CalculateQtyFast(g, "Current", 50, "Islamic"),
                    IslmCd50Leaves = CalculateLeavesFast(g, "Current", 50, "Islamic"),
                    ConCd100Books = CalculateQtyFast(g, "Current", 100, "CONV"),
                    ConCd100Leaves = CalculateLeavesFast(g, "Current", 100, "CONV"),

                    ConPo50Books = CalculateQtyFast(g, "PO", 50, "CONV"),
                    ConPo50Leaves = CalculateLeavesFast(g, "PO", 50, "CONV"),
                    ConPo100Books = CalculateQtyFast(g, "Payment Order", 100, "CONV"),
                    ConPo100Leaves = CalculateLeavesFast(g, "Payment Order", 100, "CONV"),

                    ConCc50Books = CalculateQtyFast(g, "Cash Credit", 50, "CONV"),
                    ConCc50Leaves = CalculateLeavesFast(g, "Cash Credit", 50, "CONV"),
                    ConCc100Books = CalculateQtyFast(g, "Cash Credit", 100, "CONV"),
                    ConCc100Leaves = CalculateLeavesFast(g, "Cash Credit", 100, "CONV"),

                    ConSba10Books = CalculateQtyFast(g, "SBA", 10, "CONV"),
                    ConSba10Leaves = CalculateLeavesFast(g, "SBA", 10, "CONV"),
                    ConMsa10Books = CalculateQtyFast(g, "MSA", 10, "CONV"),
                    ConMsa10Leaves = CalculateLeavesFast(g, "MSA", 10, "CONV"),
                    ConMsa20Books = CalculateQtyFast(g, "MSA", 20, "CONV"),
                    ConMsa20Leaves = CalculateLeavesFast(g, "MSA", 20, "CONV"),
                    ConMsd10Books = CalculateQtyFast(g, "MSD", 10, "CONV"),
                    ConMsd10Leaves = CalculateLeavesFast(g, "MSD", 10, "CONV"),
                    ConMsd50Books = CalculateQtyFast(g, "MSD", 50, "CONV"),
                    ConMsd50Leaves = CalculateLeavesFast(g, "MSD", 50, "CONV"),


                    ConCda25Books = CalculateQtyFast(g, "CDA", 25, "CONV"),
                    ConCda25Leaves = CalculateLeavesFast(g, "CDA", 25, "CONV"),
                    ConAcd25Books = CalculateQtyFast(g, "ACD", 25, "CONV"),
                    ConAcd25Leaves = CalculateLeavesFast(g, "ACD", 25, "CONV"),
                    ConAcd50Books = CalculateQtyFast(g, "ACD", 50, "CONV"),
                    ConAcd50Leaves = CalculateLeavesFast(g, "ACD", 50, "CONV"),
                    ConAcd100Books = CalculateQtyFast(g, "ACD", 100, "CONV"),
                    ConAcd100Leaves = CalculateLeavesFast(g, "ACD", 100, "CONV"),
                    ConAwca20Books = CalculateQtyFast(g, "AWCA", 20, "CONV"),
                    ConAwca20Leaves = CalculateLeavesFast(g, "AWCA", 20, "CONV"),
                    ConAwca50Books = CalculateQtyFast(g, "AWCA", 50, "CONV"),
                    ConAwca50Leaves = CalculateLeavesFast(g, "AWCA", 50, "CONV"),
                    ConAwca100Books = CalculateQtyFast(g, "AWCA", 100, "CONV"),
                    ConAwca100Leaves = CalculateLeavesFast(g, "AWCA", 100, "CONV"),
                    ConAwcd25Books = CalculateQtyFast(g, "AWCD", 25, "CONV"),
                    ConAwcd25Leaves = CalculateLeavesFast(g, "AWCD", 25, "CONV"),
                    ConSnd25Books = CalculateQtyFast(g, "SND", 25, "CONV"),
                    ConSnd25Leaves = CalculateLeavesFast(g, "SND", 25, "CONV"),
                    ConSnd50Books = CalculateQtyFast(g, "SND", 50, "CONV"),
                    ConSnd50Leaves = CalculateLeavesFast(g, "SND", 50, "CONV"),
                    ConSnd100Books = CalculateQtyFast(g, "SND", 100, "CONV"),
                    ConSnd100Leaves = CalculateLeavesFast(g, "SND", 100, "CONV"),
                    ConSna25Books = CalculateQtyFast(g, "SNA", 25, "CONV"),
                    ConSna25Leaves = CalculateLeavesFast(g, "SNA", 25, "CONV"),
                    ConMsnd25Books = CalculateQtyFast(g, "MSND", 25, "CONV"),
                    ConMsnd25Leaves = CalculateLeavesFast(g, "MSND", 25, "CONV"),
                    ConPoa50Books = CalculateQtyFast(g, "POA", 50, "CONV"),
                    ConPoa50Leaves = CalculateLeavesFast(g, "POA", 50, "CONV"),
                    ConPoi50Books = CalculateQtyFast(g, "POI", 50, "CONV"),
                    ConPoi50Leaves = CalculateLeavesFast(g, "POI", 50, "CONV"),

                    ConFdr50Books = CalculateQtyFast(g, "FDR", 50, "CONV"),
                    ConFdr50Leaves = CalculateLeavesFast(g, "FDR", 50, "CONV"),
                    ConFdr100Books = CalculateQtyFast(g, "FDR", 100, "CONV"),
                    ConFdr100Leaves = CalculateLeavesFast(g, "FDR", 100, "CONV"),
                    ConMtdr25Books = CalculateQtyFast(g, "MTDR", 25, "CONV"),
                    ConMtdr25Leaves = CalculateLeavesFast(g, "MTDR", 25, "CONV"),
                    ConMtdr50Books = CalculateQtyFast(g, "MTDR", 50, "CONV"),
                    ConMtdr50Leaves = CalculateLeavesFast(g, "MTDR", 50, "CONV"),

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
    private static int CalculateQtyFast(IEnumerable<dynamic> group, string? chequeType, int leaves, string? accFlag=null,int? bankId=null)
    {
        if (!string.IsNullOrEmpty(accFlag) && bankId !=null)
        {
            return group
                .Where(x => x.Leaves == leaves && x.AccFlag == accFlag)
                .Sum(x => x.BookQty);
        } 
        else
        {
            return group
                .Where(x => x.ChequeType == chequeType && x.Leaves == leaves && x.AccFlag == accFlag)
                .Sum(x => x.BookQty);
        }
    }
    private static int CalculateQtyFastIBBL(IEnumerable<dynamic> group, string? chequeType, int leaves, string? accFlag=null)
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
    private static int? CalculateLeavesFast(IGrouping<dynamic, dynamic> group, string? chequeType, int leaves, string? accFlag = null, int? bankId = null)
    {
        if (!string.IsNullOrEmpty(accFlag) && bankId != null)
        {
            return group
               .Where(x => x.Leaves == leaves && x.AccFlag == accFlag)
               .Sum(x => (int?)x.BookQty * (int?)x.Leaves);
        }
        else
        {
            return group
                .Where(x => x.ChequeType == chequeType && x.Leaves == leaves && x.AccFlag == accFlag)
                .Sum(x => (int?)x.BookQty * (int?)x.Leaves);
        }
    }

  
}
