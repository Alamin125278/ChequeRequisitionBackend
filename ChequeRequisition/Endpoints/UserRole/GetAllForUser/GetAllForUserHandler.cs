using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.UserRole;
using ChequeRequisiontService.Core.Interfaces.Repositories.IUserRole;

namespace ChequeRequisiontService.Endpoints.UserRole.GetAllForUser;
public record GetAllForUserQuery() : IQuery<GetAllForUserResponse>;
public record GetAllForUserResponse(IEnumerable<UserRoleDto> UserRoleDtos);

public class GetAllForUserHandler(IUserRoleRepo userRoleRepo,AuthenticatedUserInfo authenticatedUserInfo) : IQueryHandler<GetAllForUserQuery, GetAllForUserResponse>
{
    private readonly IUserRoleRepo _userRoleRepo = userRoleRepo;
    public async Task<GetAllForUserResponse> Handle(GetAllForUserQuery request, CancellationToken cancellationToken)
    {
        var bankId = authenticatedUserInfo.BankId; // Assuming AuthenticatedUserInfo has a BankId property
        var userRoles = await _userRoleRepo.GetAllAsync(bankId,cancellationToken);
        return new GetAllForUserResponse(userRoles);
    }
}
