using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.UserMenuPermission;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.UserMenusPermission.GetUserMenusPermissions;
public record GetUserMenusPermissionsQuery(int UserId) : IQuery<GetUserMenusPermissionsRes>;
public record GetUserMenusPermissionsRes(List<UserMenusPermissionsDto> UserMenusPermissions);

public class GetUserMenusPermissionsHandler(IUserMenuPermissionRepo userMenuPermissionRepo) : IQueryHandler<GetUserMenusPermissionsQuery, GetUserMenusPermissionsRes>
{
    public async Task<GetUserMenusPermissionsRes> Handle(GetUserMenusPermissionsQuery request, CancellationToken cancellationToken)
    {
        var userMenusPermissions = await userMenuPermissionRepo.GetUserMenusPermissionsByUserIdAsync(request.UserId, cancellationToken);
        return new GetUserMenusPermissionsRes(userMenusPermissions);
    }
}
