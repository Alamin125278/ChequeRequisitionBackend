using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.Branch;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Branch.GetAllBranch
{
    public record GetAllBranchQuery(int Skip = 0, int Limit = 10, string? Search = null, string? IsActive = null) : IQuery<GetAllBranchResult>;
   public record GetAllBranchResult(string Message, IEnumerable<BranchDto> BranchDtos,int TotalBranch);
    public class GetAllBranchHandler(IBranchRepo branchRepo, AuthenticatedUserInfo authenticatedUserInfo) :IQueryHandler<GetAllBranchQuery, GetAllBranchResult>
    {
        private readonly IBranchRepo _branchRepo = branchRepo;
        public async Task<GetAllBranchResult> Handle(GetAllBranchQuery request, CancellationToken cancellationToken)
        {
            bool? ActiveStatus = null;
            if (request.IsActive == "Active")
            {
                ActiveStatus = true;
            }
            else if (request.IsActive == "InActive")
            {
                ActiveStatus = false;
            }

            var bankId = authenticatedUserInfo.BankId;
            var totalCount = await _branchRepo.GetAllCountAsync(request.Search, bankId, ActiveStatus, cancellationToken);

            var branches = await _branchRepo.GetAllAsync(bankId,request.Skip, request.Limit, request.Search,ActiveStatus, cancellationToken);
            return new GetAllBranchResult("Retreiving Branches Success.", branches,totalCount);
        }
    }
}
