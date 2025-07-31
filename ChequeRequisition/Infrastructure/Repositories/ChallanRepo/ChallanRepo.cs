using ChequeRequisiontService.Core.Dto.Branch;
using ChequeRequisiontService.Core.Dto.Challan;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.DbContexts;
using ChequeRequisiontService.Models.CRDB;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Math;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ChequeRequisiontService.Infrastructure.Repositories.ChallanRepo;

public class ChallanRepo(CRDBContext cRDBContext) : IChallanRepo
{
    private CRDBContext _cRDBContext = cRDBContext;
    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return _cRDBContext.Database.BeginTransactionAsync(cancellationToken);
    }
    public async Task<int> AddChallanAsync(ChallanDto challanDto, int UserId, CancellationToken cancellation = default)
    {
        try
        {
            var data = challanDto.Adapt<Challan>();
            data.CreatedAt = DateTime.UtcNow;
            data.CreatedBy = UserId;
            await _cRDBContext.Challans.AddAsync(data, cancellation);
            var result = await _cRDBContext.SaveChangesAsync(cancellation);
            if (result > 0)
            {
                return data.Id;
            }
            throw new Exception("Failed to create challan");
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("Database update error: " + (ex.InnerException?.Message ?? ex.Message), ex);
        }
    }

    public async Task<int> AddChallanRequisitionAsync(ChallanTrackingDto challanTrackingDto, int UserId, CancellationToken cancellation = default)
    {
        var data = challanTrackingDto.Adapt<ChallanDetail>();
        data.CreatedAt = DateTime.UtcNow;
        data.CreatedBy = UserId;
        try
        {
            await _cRDBContext.ChallanDetails.AddAsync(data, cancellation);
            var result = await _cRDBContext.SaveChangesAsync(cancellation);
            if (result > 0)
            {
                return data.Id;
            }
            throw new Exception("Failed to create challan requisition");
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("Database update error: " + (ex.InnerException?.Message ?? ex.Message), ex);
        }
    }

    public async Task<List<ChallanExportDto>> GetChallanExportDataAsync(List<int> challanIds, CancellationToken cancellationToken)
    {
        var query = from challan in _cRDBContext.Challans.AsNoTracking()
                    join tracking in _cRDBContext.ChallanDetails on challan.Id equals tracking.ChallanId
                    join requisition in _cRDBContext.ChequeBookRequisitions on tracking.RequisitionItemId equals requisition.Id
                    join homeBranch in _cRDBContext.Branches on requisition.BranchId equals homeBranch.Id
                    join reBranch in _cRDBContext.Branches on challan.ReceivingBranch equals reBranch.Id
                    join bank in _cRDBContext.Banks on requisition.BankId equals bank.Id
                    join vendor in _cRDBContext.Vendors on requisition.VendorId equals vendor.Id
                    join courier in _cRDBContext.Couriers on requisition.CourierCode equals courier.CourierCode
                    where challanIds.Contains(challan.Id)
                    select new
                    {
                        challan.ChallanNumber,
                        challan.ChallanDate,
                        courier.CourierName,
                        bank.BankName,
                        vendor.VendorName,
                        HomeBranchName = homeBranch.BranchName,
                        ChallanBranchName = reBranch.BranchName,
                        requisition.CusAddress,
                        requisition.AgentNum,
                        requisition.IsAgent,
                        Item = new ChallanItemDto
                        {
                            ItemId = requisition.Id,
                            AccountNo = requisition.AccountNo,
                            AccountName = requisition.AccountName,
                            StartNo = requisition.StartNo,
                            EndNo = requisition.EndNo,
                            ChequeType = requisition.ChequeType,
                            BookQty = requisition.BookQty,
                            Leaves = requisition.Leaves,
                            Serverity = requisition.Serverity,
                            BranchName = homeBranch.BranchName
                        }
                    };

        var rawData = await query.ToListAsync(cancellationToken);

        var grouped = rawData
    .GroupBy(x => x.ChallanNumber)
    .Select(g => new ChallanExportDto
    {
        ChallanNumber = g.Key,
        ChallanDate = g.First().ChallanDate.ToString(),
        CourierName = g.First().CourierName,
        BankName = g.First().BankName,
        VendorName = g.First().VendorName,
        ReceivingBranchName = g.First().ChallanBranchName,
        AgentNum = g.First().AgentNum,
        CusAddress = g.First().CusAddress,
        IsAgent = g.First().IsAgent??false,
        Items = g.Select(x => x.Item).ToList()
    })
    .ToList();


        return grouped;
    }

    public async Task<IEnumerable<ChallanDto>> GetAllAsync(
        int? BankId,
        int? BranchId,
        int? VendorId,
        DateOnly? RequestDate,
        int Skip = 0,
        int Limit = 10,
        string? Search = null,
        CancellationToken cancellationToken = default)
    {

        var requisitionCounts = await _cRDBContext.ChallanDetails
    .GroupBy(cd => cd.ChallanId)
    .Select(g => new { ChallanId = g.Key, Count = g.Count() })
    .ToDictionaryAsync(x => x.ChallanId, x => x.Count, cancellationToken);

        var query = from challan in _cRDBContext.Challans.AsNoTracking()
                    join tracking in _cRDBContext.ChallanDetails on challan.Id equals tracking.ChallanId
                    join requisition in _cRDBContext.ChequeBookRequisitions on tracking.RequisitionItemId equals requisition.Id
                    join reBranch in _cRDBContext.Branches on challan.ReceivingBranch equals reBranch.Id
                    join bank in _cRDBContext.Banks on requisition.BankId equals bank.Id
                    join vendor in _cRDBContext.Vendors on requisition.VendorId equals vendor.Id
                    join courier in _cRDBContext.Couriers on requisition.CourierCode equals courier.CourierCode
                    where
                        (BankId == null || requisition.BankId == BankId) &&
                        (BranchId == null || challan.ReceivingBranch == BranchId) &&
                        (VendorId == null || requisition.VendorId == VendorId) &&
                        (RequestDate == null || challan.ChallanDate == RequestDate) &&
                        (string.IsNullOrEmpty(Search) || challan.ChallanNumber.Contains(Search))
                    group new { challan, requisition, bank, vendor, courier, reBranch } by challan.Id into g
                    select new ChallanDto
                    {
                        Id = g.Key,
                        ChallanNumber = g.Select(x => x.challan.ChallanNumber).FirstOrDefault(),
                        ChallanDate = g.Select(x => x.challan.ChallanDate).FirstOrDefault(),
                        ReceivingBranch = g.Select(x => x.challan.ReceivingBranch).FirstOrDefault(),
                        BankName = g.Select(x => x.bank.BankName).FirstOrDefault(),
                        VendorName = g.Select(x => x.vendor.VendorName).FirstOrDefault(),
                        CourierName = g.Select(x => x.courier.CourierName).FirstOrDefault(),
                        ReceivingBranchName = g.Select(x => x.reBranch.BranchName).FirstOrDefault(),
                        RequisitionCount = _cRDBContext.ChallanDetails.Count(cd => cd.ChallanId == g.Key)
                    };

        var data = await query
            .OrderByDescending(x => x.ChallanDate)
            .Skip(Skip)
            .Take(Limit)
            .ToListAsync(cancellationToken);

        return data;
    }

    public async Task<int> GetAllCountAsync(int? BankId, int? BranchId, int? VendorId, DateOnly? RequestDate, string? Search = null, CancellationToken cancellationToken = default)
    {
        var count = await(
         from challan in _cRDBContext.Challans.AsNoTracking()
         join tracking in _cRDBContext.ChallanDetails on challan.Id equals tracking.ChallanId
         join requisition in _cRDBContext.ChequeBookRequisitions on tracking.RequisitionItemId equals requisition.Id
         where
             (BankId == null || requisition.BankId == BankId) &&
             (BranchId == null || challan.ReceivingBranch == BranchId) &&
             (VendorId == null || requisition.VendorId == VendorId) &&
             (RequestDate == null || challan.ChallanDate == RequestDate) &&
             (string.IsNullOrEmpty(Search) || challan.ChallanNumber.Contains(Search))
         select challan.Id
     )
     .Distinct()
     .CountAsync(cancellationToken);

        return count;
    }

    public async Task<IEnumerable<ChallanItemDto>> GetAllItemAsync(int Id, CancellationToken cancellationToken = default)
    {
        var data = await (from challan in _cRDBContext.Challans.AsNoTracking()
                    join tracking in _cRDBContext.ChallanDetails on challan.Id equals tracking.ChallanId
                    join requisition in _cRDBContext.ChequeBookRequisitions on tracking.RequisitionItemId equals requisition.Id
                    join branch in _cRDBContext.Branches on requisition.BranchId equals branch.Id
                    where challan.Id==Id
                    select new ChallanItemDto
                    {
                        ItemId=requisition.Id,
                        AccountNo=requisition.AccountNo,
                        AccountName=requisition.AccountName,
                        StartNo=requisition.StartNo,
                        EndNo=requisition.EndNo,
                        BookQty=requisition.BookQty,
                        Serverity=requisition.Serverity,
                        BranchName=branch.BranchName,
                        ChequeType=requisition.ChequeType,
                        Leaves= requisition.Leaves
                    }).ToListAsync(cancellationToken);
        return data;
    }

    public async Task<int> GetAllItemCountAsync(int Id, CancellationToken cancellationToken = default)
    {
        var count = await(from challan in _cRDBContext.Challans.AsNoTracking()
                         join tracking in _cRDBContext.ChallanDetails on challan.Id equals tracking.ChallanId
                         join requisition in _cRDBContext.ChequeBookRequisitions on tracking.RequisitionItemId equals requisition.Id
                         join branch in _cRDBContext.Branches on requisition.BranchId equals branch.Id
                         where challan.Id == Id
                         select requisition.Id
                         )
                         .CountAsync(cancellationToken);
        return count;
    }
}
