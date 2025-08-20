using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Branch.GetBranchByCode;
public record GetBranchByCodeQuery(int BankId,string BranchCode,string BranchName) : IQuery<GetBranchByCodeRes>;
public record class GetBranchByCodeRes(bool HasBranch,int BranchId);
public class GetBranchByCodeHandler(IBranchRepo branchRepo) : IQueryHandler<GetBranchByCodeQuery, GetBranchByCodeRes>
{
    public async Task<GetBranchByCodeRes> Handle(GetBranchByCodeQuery request, CancellationToken cancellationToken)
    {
        var branch = await branchRepo.GetIdAsync(request.BankId,request.BranchName,request.BranchCode, cancellationToken);
        return new GetBranchByCodeRes(branch != 0, branch);
    }
}

