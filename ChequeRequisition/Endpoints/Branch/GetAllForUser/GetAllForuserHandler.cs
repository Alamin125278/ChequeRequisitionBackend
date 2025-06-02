using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Branch;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Branch.GetAllForUser;
public record GetAllForuserQuery(int BankId) : IQuery<GetAllForuserResponse>;
public record class GetAllForuserResponse(IEnumerable<BranchDto> BranchDtos);

public class GetAllForuserHandler(IBranchRepo branchRepo) : IQueryHandler<GetAllForuserQuery, GetAllForuserResponse>
{
    public async Task<GetAllForuserResponse> Handle(GetAllForuserQuery request, CancellationToken cancellationToken)
    {
        var branches = await branchRepo.GetAllAsync(request.BankId, cancellationToken);
        return new GetAllForuserResponse(branches);
    }
}
