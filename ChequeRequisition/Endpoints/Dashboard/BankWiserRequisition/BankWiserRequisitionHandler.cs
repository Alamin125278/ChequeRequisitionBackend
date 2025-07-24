using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.Dashboard;
using ChequeRequisiontService.Core.Interfaces.Repositories.Dashboard;

namespace ChequeRequisiontService.Endpoints.Dashboard.BankWiserRequisition;
public record GetBankWiserRequisitionQuery() : IQuery<BankWiserRequisitionResponse>;

public record BankWiserRequisitionResponse(IEnumerable<BankWiserRequisitonDto>? BankWiserRequisitions);

public class BankWiserRequisitionHandler(
    IDashboardRepo dashboardRepo,AuthenticatedUserInfo authenticatedUserInfo) : IQueryHandler<GetBankWiserRequisitionQuery, BankWiserRequisitionResponse>
{
    public async Task<BankWiserRequisitionResponse> Handle(GetBankWiserRequisitionQuery request, CancellationToken cancellationToken)
    {
        var bankWiserRequisitions = await dashboardRepo.GetBankWiseRequisitionAsync(authenticatedUserInfo.VendorId, cancellationToken);
        return new BankWiserRequisitionResponse(bankWiserRequisitions);
    }
}

