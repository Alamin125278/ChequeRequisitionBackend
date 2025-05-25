using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.UserRole;
using ChequeRequisiontService.Core.Interfaces.Repositories.IUserRole;

namespace ChequeRequisiontService.Endpoints.UserRole.GetAllUserRole;
public record GetAllRoleQuery(int Skip=0,int Limit=10, string? Search = null):IQuery<GetAllRoleResponse>;
public record GetAllRoleResponse(string Message, IEnumerable<UserRoleDto> UserRoleDtos);
public class GetAllRoleHandler(IUserRoleRepo userRoleRepo):IQueryHandler<GetAllRoleQuery, GetAllRoleResponse>
{
    private readonly IUserRoleRepo _userRoleRepo = userRoleRepo;
    public async Task<GetAllRoleResponse> Handle(GetAllRoleQuery request, CancellationToken cancellationToken)
    {
        var userRoles = await _userRoleRepo.GetAllAsync(request.Skip, request.Limit, request.Search, cancellationToken);
        return new GetAllRoleResponse("Retrieving User Roles Success.", userRoles);
    }
}
