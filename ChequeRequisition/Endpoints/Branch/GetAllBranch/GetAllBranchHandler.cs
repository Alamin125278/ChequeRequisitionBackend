using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.BranchDto;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Branch.GetAllBranch
{
    public record GetAllBranchQuery(int Skip = 0, int Limit = 10, string? Search = null) : IQuery<GetAllBranchResult>;
   public record GetAllBranchResult(string Message, IEnumerable<BranchDto> BranchDtos);
    public class GetAllBranchHandler(IBranchRepo branchRepo):IQueryHandler<GetAllBranchQuery, GetAllBranchResult>
    {
        private readonly IBranchRepo _branchRepo = branchRepo;
        public async Task<GetAllBranchResult> Handle(GetAllBranchQuery request, CancellationToken cancellationToken)
        {
            var branches = await _branchRepo.GetAllAsync(request.Skip, request.Limit, request.Search, cancellationToken);
            return new GetAllBranchResult("Retreiving Branches Success.", branches);
        }
    }
}
