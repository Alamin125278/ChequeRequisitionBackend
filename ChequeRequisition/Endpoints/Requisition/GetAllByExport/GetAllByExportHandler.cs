using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.Requisition;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Requisition.GetAllByExport;
public record  GetAllByExportQuery(
    int? BankId = null,
    int? BranchId = null,
    int? Severity = null,
    string? RequestDate = null,
    string? Search = null,
    bool? IsAgent=null
) : IQuery<GetAllByExportResult>;

public record GetAllByExportResult(
    IEnumerable<RequisitionDto> Requisitions
);

public class GetAllByExportHandler(IRequisitonRepo requisitonRepo,AuthenticatedUserInfo authenticatedUserInfo) : IQueryHandler<GetAllByExportQuery, GetAllByExportResult>
{
    public async Task<GetAllByExportResult> Handle(GetAllByExportQuery request, CancellationToken cancellationToken)
    {
       
        
        DateOnly? requestDate = null;
        if (!string.IsNullOrWhiteSpace(request.RequestDate))
        {
            requestDate = DateOnly.Parse(request.RequestDate);
        }
        var requisitions = await requisitonRepo.GetAllAsync(
            3,
            request.BankId,
            request.BranchId,
            authenticatedUserInfo.VendorId,
            request.Severity,
            requestDate,
            request.Search,
            request.IsAgent,
            cancellationToken
        );
        return new GetAllByExportResult(requisitions);
    }
}

