using ChequeRequisiontService.Core.Dto.Menu;
using ChequeRequisiontService.Core.Dto.UserMenuPermission;
using ChequeRequisiontService.Models.CRDB;

namespace ChequeRequisiontService.Core.Interfaces.Repositories
{
    public interface IUserMenuPermissionRepo:IGenericRepository<UserMenuPermissionDto>
    {
        Task<List<MenuDto>> GetMenusByUserIdAsync(int userId,CancellationToken cancellationToken=default);
        Task<bool> DeleteMenusByUserIdAsync(int userId,int DeleteById,CancellationToken cancellationToken=default);
        bool CheckRoutePermission(List<MenuDto> menus,string path, CancellationToken cancellationToken = default);
        Task<List<UserMenusPermissionsDto>> GetUserMenusPermissionsByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task BulkCreateAsync(
        IEnumerable<UserMenuPermission> permissions,
        CancellationToken cancellationToken);

        Task BulkSoftDeleteAsync(
            int userId,
            IEnumerable<int> menuIds,
            int actionBy,
            CancellationToken cancellationToken);
    }
}
