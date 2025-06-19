using ChequeRequisiontService.Core.Dto.Ftp;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.DbContexts;
using ChequeRequisiontService.Models.CRDB;
using Mapster;

namespace ChequeRequisiontService.Infrastructure.Repositories.FtpImportExcel;

public class RequisitionTrackingRepo(CRDBContext cRDBContext) : IRequisitionTrackingRepo
{
    private readonly CRDBContext _cRDBContext = cRDBContext;
    public async Task InsertTrackingAsync(List<RequisitionImportTrackingDto> trackings, CancellationToken cancellationToken = default)
    {
        var entities= trackings.Adapt<List<FtpRequisitionTracking>>();
       await _cRDBContext.FtpRequisitionTrackings.AddRangeAsync(entities, cancellationToken);
        var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
        if (result <= 0)
        {
            throw new Exception("Failed to insert requisition tracking data");
        }
    }
}
