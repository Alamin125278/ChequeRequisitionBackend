using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.Requisition;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Requisition.GetAllOrderRequisition;
public record GetAllOrderRequisitionQuery(
    int Status,
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
    IRequisitonRepo requisitionRepo,AuthenticatedUserInfo authenticatedUserInfo
) : IQueryHandler<GetAllOrderRequisitionQuery, GetAllOrderRequisitionResult>
{
    public async Task<GetAllOrderRequisitionResult> Handle(GetAllOrderRequisitionQuery request, CancellationToken cancellationToken)
    {
        DateOnly? requestDate = null;
        int? bankId;
        int? branchId;
        if (!string.IsNullOrWhiteSpace(request.RequestDate))
        {
         requestDate = DateOnly.Parse(request.RequestDate);
        }
        if (authenticatedUserInfo.BankId !=null)
        {
            bankId = authenticatedUserInfo.BankId;
        }
        else
        {
            bankId = request.BankId;
        }
        if (authenticatedUserInfo.BranchId !=null)
        {
            branchId = authenticatedUserInfo.BranchId;
        }
        else
        {
            branchId = request.BranchId;
        }

            var requisitions = await requisitionRepo.GetAllAsync(request.Status, bankId, branchId, authenticatedUserInfo.VendorId, request.Severity, requestDate, request.Skip, request.Limit, request.Search, cancellationToken);
        var totalCount = await requisitionRepo.GetAllCountAsync(
            request.Status,
            bankId,
            branchId,
            authenticatedUserInfo.VendorId,
            request.Severity,
            requestDate,
            request.Search,
            cancellationToken
        );
        return new GetAllOrderRequisitionResult(requisitions, totalCount);
    }
}
