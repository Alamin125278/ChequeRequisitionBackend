using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.UserMenuPermission;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.UserMenu.GetUserMenuPermission;
public record GetUserMenuPermissionQuery(int Id):IQuery<GetUserMenuPermissionResponse>;
public record GetUserMenuPermissionResponse(UserMenuPermissionDto? UserMenuPermission);

public class GetUserMenuPermissionHandler(IUserMenuPermissionRepo userMenuPermissionRepo):IQueryHandler<GetUserMenuPermissionQuery, GetUserMenuPermissionResponse>
{
    public async Task<GetUserMenuPermissionResponse> Handle(GetUserMenuPermissionQuery request, CancellationToken cancellationToken)
    {
        var userMenuPermission = await userMenuPermissionRepo.GetByIdAsync(request.Id, cancellationToken);
        return userMenuPermission == null ? throw new NotFoundException($"User Menu Permission with ID {request.Id} not found.") : new GetUserMenuPermissionResponse(userMenuPermission);
    }
}

