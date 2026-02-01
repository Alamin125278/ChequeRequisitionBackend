using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.Endpoints.DefaultMenuPermission.GetAllDefaultMenu;

namespace ChequeRequisiontService.Endpoints.DefaultMenuPermission.DefaultMenusByRole
{
    public record GetAllDefaultMenusByRoleQuery(int RoleId) : IQuery<GetAllDefaultMenusByRoleRes>;
    public record GetAllDefaultMenusByRoleRes(string Message, IEnumerable<Core.Dto.DefaultMenuPermission.DefaultMenuPermissionByRoleDto> DefaultMenuPermissionByRole );
    public class DefaultMenusByRoleHandler(IDefaultMenuPermisionRepo defaultMenuPermisionRepo) : IQueryHandler<GetAllDefaultMenusByRoleQuery, GetAllDefaultMenusByRoleRes>
    {
        public async Task<GetAllDefaultMenusByRoleRes> Handle(GetAllDefaultMenusByRoleQuery request, CancellationToken cancellationToken)
        {
          var result = await defaultMenuPermisionRepo.GetAllMenuByRoleAsync(request.RoleId, cancellationToken);
            return new GetAllDefaultMenusByRoleRes("Default Menus by Role retrieved successfully",result);
        }
    }
}
