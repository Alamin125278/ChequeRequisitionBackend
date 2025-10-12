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
using System.Linq;

namespace ChequeRequisiontService.Infrastructure.Repositories.ChallanRepo;

public class ChallanRepo(CRDBContext cRDBContext) : IChallanRepo
{
    private CRDBContext _cRDBContext = cRDBContext;
    private static int ExtractSerialFromChallanNumber(string challanNumber)
    {
        var parts = challanNumber?.Split('-');
        if (parts != null && parts.Length == 3 && int.TryParse(parts[2], out int serial))
        {
            return serial;
        }
        return 0;
    }

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
                        CourierPhone=courier.Phone,
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
        CourierPhone = g.First().CourierPhone,
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
        // Step 1: Filtered Challan IDs with pagination
        var filteredChallanIds = await (
            from challan in _cRDBContext.Challans
            join detail in _cRDBContext.ChallanDetails on challan.Id equals detail.ChallanId
            join requisition in _cRDBContext.ChequeBookRequisitions on detail.RequisitionItemId equals requisition.Id
            where
                (BankId == null || requisition.BankId == BankId) &&
                (BranchId == null || challan.ReceivingBranch == BranchId) &&
                (VendorId == null || requisition.VendorId == VendorId) &&
                (RequestDate == null || challan.ChallanDate == RequestDate) &&
                (string.IsNullOrEmpty(Search) || challan.ChallanNumber.Contains(Search))
            select challan.Id
        )
        .Distinct()
        .OrderByDescending(id => id)
        .Skip(Skip)
        .Take(Limit)
        .ToListAsync(cancellationToken);

        if (!filteredChallanIds.Any())
            return Enumerable.Empty<ChallanDto>();

        // Step 2: Preload Requisition Counts (once only)
        var requisitionCounts = await _cRDBContext.ChallanDetails
            .Where(cd => filteredChallanIds.Contains(cd.ChallanId.Value))
            .GroupBy(cd => cd.ChallanId)
            .Select(g => new { ChallanId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.ChallanId, x => x.Count, cancellationToken);

        // Step 3: Load all required data
        var query = from challan in _cRDBContext.Challans.AsNoTracking()
                    join detail in _cRDBContext.ChallanDetails on challan.Id equals detail.ChallanId
                    join requisition in _cRDBContext.ChequeBookRequisitions on detail.RequisitionItemId equals requisition.Id
                    join branch in _cRDBContext.Branches on challan.ReceivingBranch equals branch.Id
                    join bank in _cRDBContext.Banks on requisition.BankId equals bank.Id
                    join vendor in _cRDBContext.Vendors on requisition.VendorId equals vendor.Id
                    join courier in _cRDBContext.Couriers on requisition.CourierCode equals courier.CourierCode
                    where filteredChallanIds.Contains(challan.Id)
                    select new
                    {
                        challan.Id,
                        challan.ChallanNumber,
                        challan.ChallanDate,
                        challan.ReceivingBranch,
                        branch.BranchName,
                        bank.BankName,
                        vendor.VendorName,
                        courier.CourierName
                    };

        // Step 4: Final projection with distinct ChallanIds (remove duplicates from join)
        var challanData = await query
            .Distinct()
            .ToListAsync(cancellationToken);

        var result = challanData.Select(x => new ChallanDto
        {
            Id = x.Id,
            ChallanNumber = x.ChallanNumber,
            ChallanDate = x.ChallanDate,
            ReceivingBranch = x.ReceivingBranch,
            ReceivingBranchName = x.BranchName,
            BankName = x.BankName,
            VendorName = x.VendorName,
            CourierName = x.CourierName,
            RequisitionCount = requisitionCounts.ContainsKey(x.Id) ? requisitionCounts[x.Id] : 0
        }).OrderByDescending(x => x.Id); 

        return result;
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

    public async Task<int> GetChallanNumber(int BankId, CancellationToken cancellationToken = default)
    {
        var lastChallanNumber = await(from r in _cRDBContext.ChequeBookRequisitions
                                      join cd in _cRDBContext.ChallanDetails on r.Id equals cd.RequisitionItemId
                                      join c in _cRDBContext.Challans on cd.ChallanId equals c.Id
                                      where r.BankId == BankId
                                      orderby c.Id descending
                                      select c.ChallanNumber)
                                 .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrEmpty(lastChallanNumber))
            return 0;

        return ExtractSerialFromChallanNumber(lastChallanNumber);
    }
}
