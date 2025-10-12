using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Branch;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Branch.GetBranchByCode;
public record GetBranchByCodeQuery(int BankId,string BranchCode,string BranchName) : IQuery<GetBranchByCodeRes>;
public record class GetBranchByCodeRes(bool HasBranch, BranchDto? Branch);
public class GetBranchByCodeHandler(IBranchRepo branchRepo) : IQueryHandler<GetBranchByCodeQuery, GetBranchByCodeRes>
{
    public async Task<GetBranchByCodeRes> Handle(GetBranchByCodeQuery request, CancellationToken cancellationToken)
    {
        var branch = await branchRepo.GetIdAsync(request.BankId,request.BranchName,request.BranchCode, cancellationToken);
        if(branch== null) return new GetBranchByCodeRes(false, null);
        return new GetBranchByCodeRes(true, branch);
    }
}

