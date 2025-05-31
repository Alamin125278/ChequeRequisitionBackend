using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Branch.GetBranchCount;
public record GetBranchCountQuery() : IQuery<GetBranchCountResult>;
public record GetBranchCountResult(int TotalBranch, int ActiveBranch);

public class GetBranchCountHandler(IBranchRepo branchRepo, AuthenticatedUserInfo authenticatedUserInfo):IQueryHandler<GetBranchCountQuery, GetBranchCountResult>
{
    public async Task<GetBranchCountResult> Handle(GetBranchCountQuery request, CancellationToken cancellationToken)
    {
        var totalBranch = await branchRepo.GetAllCountAsync(null,authenticatedUserInfo.BankId,null, cancellationToken);
        var activeBranch = await branchRepo.GetAllCountAsync(null,authenticatedUserInfo.BankId, true, cancellationToken);
        return new GetBranchCountResult(totalBranch, activeBranch);
    }
}
