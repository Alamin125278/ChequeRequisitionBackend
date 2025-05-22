using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.BranchDto;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Branch.GetBranch
{
    public record GetBranchQuery(int Id) : IQuery<GetBranchResult>;
    public record GetBranchResult(BranchDto? Branch);
    public class GetBranchHandler(IBranchRepo branchRepo):IQueryHandler<GetBranchQuery, GetBranchResult>
    {
        public async Task<GetBranchResult> Handle(GetBranchQuery request, CancellationToken cancellationToken)
        {
            var branch = await branchRepo.GetByIdAsync(request.Id, cancellationToken);
            if (branch == null)
            {
                throw new NotFoundException($"Branch with ID {request.Id} not found.");
            }
            return new GetBranchResult(branch);
        }
    }
}
