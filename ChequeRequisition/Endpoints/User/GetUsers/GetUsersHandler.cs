using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.Branch;
using ChequeRequisiontService.Core.Dto.User;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.Endpoints.User.GetUser;

namespace ChequeRequisiontService.Endpoints.User.GetUsers;
public record GetUsersQuery():IQuery<GetUsersRes>;

public record GetUsersRes(string Message, IEnumerable<UserDto> Users);
public class GetUsersHandler(IUserRepo userRepo, AuthenticatedUserInfo authenticatedUserInfo) : IQueryHandler<GetUsersQuery, GetUsersRes>
{
    public async Task<GetUsersRes> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users= await userRepo.GetAllAsync(authenticatedUserInfo.VendorId ?? 0, cancellationToken);
        return new GetUsersRes("Retreiving Users Success.", users);
    }
}
