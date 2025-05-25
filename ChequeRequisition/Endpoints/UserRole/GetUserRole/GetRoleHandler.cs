using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.UserRole;
using ChequeRequisiontService.Core.Interfaces.Repositories.IUserRole;

namespace ChequeRequisiontService.Endpoints.UserRole.GetUserRole;
public record GetRoleQuery(int Id) : IQuery<GetRoleResponse>;
public record GetRoleResponse(UserRoleDto? UserRole);
public class GetRoleHandler(IUserRoleRepo userRoleRepo) : IQueryHandler<GetRoleQuery, GetRoleResponse>
{
    public async Task<GetRoleResponse> Handle(GetRoleQuery request, CancellationToken cancellationToken)
    {
        var userRole = await userRoleRepo.GetByIdAsync(request.Id, cancellationToken);
        return userRole == null ? throw new NotFoundException($"User Role with ID {request.Id} not found.") : new GetRoleResponse(userRole);
    }
}
