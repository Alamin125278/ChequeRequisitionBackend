using ChequeRequisiontService.Core.Dto.BulkUploadResult;
using ChequeRequisiontService.Core.Dto.Requisition;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.DbContexts;
using ChequeRequisiontService.Models.CRDB;
using DocumentFormat.OpenXml.Spreadsheet;
using Mapster;

namespace ChequeRequisiontService.Infrastructure.Repositories.LocalFileUploadRepo;

public class LocalFileUploadRepo(CRDBContext cRDBContext) : ILocalFileUploadRepo
{
    private readonly CRDBContext _cRDBContext = cRDBContext;

    public async Task<BulkUploadResultDto> BulkUploadAsync(List<RequisitionDto> Items, int UserId, int? VendorId, CancellationToken cancellationToken = default)
    {
        using var transaction = await _cRDBContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var entities = Items.Select(dto =>
            {
                var data = dto.Adapt<ChequeBookRequisition>();
                data.Status = 3;
                data.IsDeleted = false;
                data.CreatedBy = UserId;
                data.RequestedBy = UserId;
                data.VendorId = VendorId;
                data.CreatedAt = DateTime.UtcNow;

                return data;
            }).ToList();

            await _cRDBContext.ChequeBookRequisitions.AddRangeAsync(entities, cancellationToken);
            await _cRDBContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken); // ✅ commit only if everything is successful

            return new BulkUploadResultDto
            {
                Success = true,
                ErrorMessage = null
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken); // ❌ rollback if any error                                   // Optional: log the exception
            return new BulkUploadResultDto
            {
                Success = false,
                ErrorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message, // You might want to log this or handle it differently
            };
        }
    }


    public async Task<bool> LocalFileUploadAsync(RequisitionDto requisitionDto, int userId, CancellationToken cancellationToken = default)
    {
        var data = requisitionDto.Adapt<ChequeBookRequisition>();
        data.Status = 3;
        data.IsDeleted = false;
        data.CreatedBy = userId;
        data.RequestedBy = userId;
        data.RequestDate = DateOnly.FromDateTime(DateTime.Now);
        data.CreatedAt = DateTime.UtcNow;
        await _cRDBContext.ChequeBookRequisitions.AddAsync(data, cancellationToken);
        var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
        if (result > 0)
        {
            return true;
        }
        return false;
    }
}
