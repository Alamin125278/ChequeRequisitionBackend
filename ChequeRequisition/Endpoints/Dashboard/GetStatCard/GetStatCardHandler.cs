using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.Dashboard;
using ChequeRequisiontService.Core.Interfaces.Repositories.Dashboard;

namespace ChequeRequisiontService.Endpoints.Dashboard.GetStatCard;
public record GetStatCardQuery(int? BankId=null):IQuery<GetStatCardResponse>;

public record GetStatCardResponse(IEnumerable<StatCardDto> StatCardDto);
public class GetStatCardHandler(IDashboardRepo dashboardRepo, AuthenticatedUserInfo authenticatedUserInfo):IQueryHandler<GetStatCardQuery, GetStatCardResponse>
{
    public async Task<GetStatCardResponse> Handle(GetStatCardQuery request, CancellationToken cancellationToken)
    {
        int? bankId = request.BankId ?? authenticatedUserInfo.BankId;

        var statCardDto = new List<StatCardDto>
        {
            new() {
                TotalRequisition = await dashboardRepo.GetAllCountAsync(null,bankId, authenticatedUserInfo.BranchId, authenticatedUserInfo.VendorId, cancellationToken),
                OrderedRequisition = await dashboardRepo.GetAllCountAsync(3,bankId, authenticatedUserInfo.BranchId, authenticatedUserInfo.VendorId, cancellationToken),
                ProcessingRequisition = await dashboardRepo.GetAllCountAsync(4,bankId, authenticatedUserInfo.BranchId, authenticatedUserInfo.VendorId, cancellationToken),
                DispatchedRequisition = await dashboardRepo.GetAllCountAsync(5,bankId, authenticatedUserInfo.BranchId, authenticatedUserInfo.VendorId, cancellationToken),
                DeliveredRequisition = await dashboardRepo.GetAllCountAsync(6,bankId, authenticatedUserInfo.BranchId, authenticatedUserInfo.VendorId, cancellationToken),
            }
        };
        return new GetStatCardResponse(statCardDto);
    }
}