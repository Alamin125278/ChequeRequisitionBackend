using ChequeRequisiontService.Core.Dto.Challan;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.DbContexts;
using ChequeRequisiontService.Models.CRDB;
using DocumentFormat.OpenXml.InkML;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ChequeRequisiontService.Infrastructure.Repositories;

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
                    where challanIds.Contains(challan.Id)
                    select new
                    {
                        challan.ChallanNumber,
                        challan.ChallanDate,
                        CourierName = "Pathao",
                        BankName = bank.BankName,
                        VendorName = vendor.VendorName,
                        HomeBranchName = homeBranch.BranchName,
                        ChallanBranchName = reBranch.BranchName,
                        AgentNum = requisition.AgentNum,
                        Item = new ChallanItemDto
                        {
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
        Items = g.Select(x => x.Item).ToList()
    })
    .ToList();


        return grouped;
    }

}
