using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.Menu;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.UserMenu.GetByUserMenuPermission;
public record GetByUserMenuPermissionQuery() : IQuery<GetByUserMenuPermissionResponse>;
public record GetByUserMenuPermissionResponse(List<MenuDto> Menus);

public class GetByUserMenuPermissionHandler(IUserMenuPermissionRepo userMenuPermissionRepo, AuthenticatedUserInfo authenticatedUserInfo) : IQueryHandler<GetByUserMenuPermissionQuery, GetByUserMenuPermissionResponse>
{
    public async Task<GetByUserMenuPermissionResponse> Handle(GetByUserMenuPermissionQuery request, CancellationToken cancellationToken)
    {
        var userId = authenticatedUserInfo.Id;
        if (userId <= 0)
        {
            throw new UnauthorizedAccessException("User ID not found");
        }
        var menus = await userMenuPermissionRepo.GetMenusByUserIdAsync(userId, cancellationToken);
        return new GetByUserMenuPermissionResponse(menus);
    }
}
