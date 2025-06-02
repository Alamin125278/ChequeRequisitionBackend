using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.User.GetAllUser;

public record GetAllUserQuery(int Skip=0,int Limit=10 ,string? Search = null,string? IsActive=null,int? Role=null) : IQuery<GetUserResult>;

public record GetUserResult(string Message, IEnumerable<Core.Dto.User.UserDto> UserDtos,int TotalUser);

public class GetAllUserHandler(IUserRepo userRepo,AuthenticatedUserInfo authenticatedUserInfo) : IQueryHandler<GetAllUserQuery, GetUserResult>
{
 
    private readonly IUserRepo _userRepo = userRepo;
    public async Task<GetUserResult> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
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
        var branchId = authenticatedUserInfo.BranchId;
        if(branchId != null)
        {
            bankId = 0; 
        }
        var totalCount = await _userRepo.GetAllCountAsync(bankId, branchId, request.Role, request.Search, ActiveStatus, cancellationToken);
        var users = await _userRepo.GetAllAsync(bankId,branchId,request.Role,request.Skip, request.Limit, request.Search,ActiveStatus, cancellationToken);
        return new GetUserResult("Retreiving Users Success.", users,totalCount);
    }
}
