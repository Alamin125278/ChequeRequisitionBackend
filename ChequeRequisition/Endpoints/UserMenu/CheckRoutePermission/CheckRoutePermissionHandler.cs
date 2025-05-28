using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.UserMenu.CheckRoutePermission;
public record CheckRoutePermissionQuery(string Path):IQuery<CheckRoutePermissionResult>;
public record CheckRoutePermissionResult(bool HasPermission);
public class CheckRoutePermissionHandler(IUserMenuPermissionRepo userMenuPermissionRepo, AuthenticatedUserInfo authenticatedUserInfo) : IQueryHandler<CheckRoutePermissionQuery, CheckRoutePermissionResult>
{
    public async Task<CheckRoutePermissionResult> Handle(CheckRoutePermissionQuery request, CancellationToken cancellationToken)
    {
        var userId = authenticatedUserInfo.Id;
        if (userId <= 0)
        {
            throw new UnauthorizedAccessException("User ID not found");
        }
        var menus = await userMenuPermissionRepo.GetMenusByUserIdAsync(userId, cancellationToken);
        var hasPermission =  userMenuPermissionRepo.CheckRoutePermission(menus, request.Path, cancellationToken);
        return new CheckRoutePermissionResult(hasPermission);
    }
}
