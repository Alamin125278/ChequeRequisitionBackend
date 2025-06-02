using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.User.GetUserCount;
public record GetUserCountQuery() : IQuery<GetUserCountResult>;
public record GetUserCountResult(int TotalUser, int ActiveUser);

public class GetUserCountHandler(IUserRepo userRepo,AuthenticatedUserInfo authenticatedUserInfo): IQueryHandler<GetUserCountQuery, GetUserCountResult>
{
    public async Task<GetUserCountResult> Handle(GetUserCountQuery request, CancellationToken cancellationToken)
    {
        var bankId = authenticatedUserInfo.BankId;
        var branchId = authenticatedUserInfo.BranchId;
        if (branchId != null)
        {
            bankId = null;
        }
        var totalUser = await userRepo.GetAllCountAsync(bankId, branchId,null,null,null,cancellationToken);
        var activeUser = await userRepo.GetAllCountAsync(bankId, branchId,null,null, true,cancellationToken);
        return new GetUserCountResult(totalUser, activeUser);
    }
}
