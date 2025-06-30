using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Requisition;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Requisition.GetAllOrderRequisition;
public record GetAllOrderRequisitionQuery(
    int? BankId = null,
    int? BranchId = null,
    int? Severity = null,
    string? RequestDate = null,
    int Skip = 0,
    int Limit = 10,
    string? Search = null
):IQuery<GetAllOrderRequisitionResult>;

public record GetAllOrderRequisitionResult(
    IEnumerable<RequisitionDto> Requisitions,
    int TotalCount
);

public class GetAllOrderRequisitionHandler(
    IRequisitonRepo requisitionRepo
) : IQueryHandler<GetAllOrderRequisitionQuery, GetAllOrderRequisitionResult>
{
    public async Task<GetAllOrderRequisitionResult> Handle(GetAllOrderRequisitionQuery request, CancellationToken cancellationToken)
    {
        DateOnly? requestDate = null;
        if (!string.IsNullOrWhiteSpace(request.RequestDate))
        {
         requestDate = DateOnly.Parse(request.RequestDate);
        }
       
            var requisitions = await requisitionRepo.GetAllAsync(3, request.BankId, request.BranchId, request.Severity, requestDate, request.Skip, request.Limit, request.Search, cancellationToken);
        var totalCount = await requisitionRepo.GetAllCountAsync(
            3,
            request.BankId,
            request.BranchId,
            request.Severity,
            requestDate,
            request.Search,
            cancellationToken
        );
        return new GetAllOrderRequisitionResult(requisitions, totalCount);
    }
}
