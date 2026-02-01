using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.DefaultMenuPermission.GetRoleByDefaultMenu;
public record GetRoleByDefaultMenuQuery(int RoleId, int MenuId) : IQuery<GetRoleByDefaultMenuResponse>;
public record GetRoleByDefaultMenuResponse(bool HasPermission);

public class GetRoleByDefaultMenuHandler(IDefaultMenuPermisionRepo defaultMenuPermisionRepo) : IQueryHandler<GetRoleByDefaultMenuQuery, GetRoleByDefaultMenuResponse>
{
    public async Task<GetRoleByDefaultMenuResponse> Handle(GetRoleByDefaultMenuQuery request, CancellationToken cancellationToken)
    {
        var hasPermission = await defaultMenuPermisionRepo.GetRoleByDefaultMenuAsync(request.RoleId, request.MenuId, cancellationToken);
        return new GetRoleByDefaultMenuResponse(hasPermission);
    }
}

