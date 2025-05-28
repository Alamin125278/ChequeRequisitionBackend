using ChequeRequisiontService.Core.Dto.Menu;
using ChequeRequisiontService.Core.Dto.UserMenuPermission;

namespace ChequeRequisiontService.Core.Interfaces.Repositories
{
    public interface IUserMenuPermissionRepo:IGenericRepository<UserMenuPermissionDto>
    {
        Task<List<MenuDto>> GetMenusByUserIdAsync(int userId,CancellationToken cancellationToken=default);
        bool CheckRoutePermission(List<MenuDto> menus,string path, CancellationToken cancellationToken = default);
    }
}
