using ChequeRequisiontService.Core.Dto.Requisition;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.DbContexts;
using ChequeRequisiontService.Models.CRDB;
using Mapster;

namespace ChequeRequisiontService.Infrastructure.Repositories.LocalFileUploadRepo;

public class LocalFileUploadRepo(CRDBContext cRDBContext) : ILocalFileUploadRepo
{
    private readonly CRDBContext _cRDBContext = cRDBContext;
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
