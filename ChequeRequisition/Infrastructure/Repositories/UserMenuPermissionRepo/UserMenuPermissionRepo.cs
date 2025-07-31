using ChequeRequisiontService.Core.Dto.Menu;
using ChequeRequisiontService.Core.Dto.UserMenuPermission;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.DbContexts;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace ChequeRequisiontService.Infrastructure.Repositories.UserMenuPermissionRepo
{
    public class UserMenuPermissionRepo(CRDBContext cRDBContext) : IUserMenuPermissionRepo
    {
        private readonly CRDBContext _cRDBContext = cRDBContext;

        public bool CheckRoutePermission(List<MenuDto> menus, string path, CancellationToken cancellationToken = default)
        {
            var normalizedPath = path?.Trim().ToLowerInvariant();
            var exists = menus.Any(m => m.Path?.Trim().ToLowerInvariant() == normalizedPath);
            if(exists == false)
            {
                return false;
            }
            return true;
        }

        public async Task<UserMenuPermissionDto> CreateAsync(UserMenuPermissionDto entity, int UserId, CancellationToken cancellationToken = default)
        {
           try
            { 
                var data = entity.Adapt<Models.CRDB.UserMenuPermission>();
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = UserId;
                data.IsDeleted = false;

                await _cRDBContext.UserMenuPermissions.AddAsync(data, cancellationToken);
                var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
                if (result > 0)
                {
                    return data.Adapt<UserMenuPermissionDto>();
                }
                throw new Exception("Failed to create user menu permission");
            }
            catch(DbException ex)
            {
                throw new Exception("Database update error: " + (ex.InnerException?.Message ?? ex.Message), ex);
            }
        }

        public async Task<bool> DeleteAsync(int id, int UserId, CancellationToken cancellationToken = default)
        {
            var userMenuPermission = await _cRDBContext.UserMenuPermissions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (userMenuPermission == null)
                return false;
            userMenuPermission.IsDeleted = true;
            userMenuPermission.UpdatedAt = DateTime.UtcNow;
            userMenuPermission.UpdatedBy = UserId;
            var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }

        public async Task<bool> DeleteMenusByUserIdAsync(int userId, int DeleteById, CancellationToken cancellationToken = default)
        {
            var affectedRows = await _cRDBContext.UserMenuPermissions
        .Where(x => x.UserId == userId && x.IsActive == true && x.IsDeleted != true)
        .ExecuteUpdateAsync(setters => setters
            .SetProperty(x => x.IsDeleted, true)
            .SetProperty(x => x.UpdatedBy, DeleteById)
            .SetProperty(x => x.UpdatedAt, DateTime.UtcNow), // optional
            cancellationToken);

            return affectedRows > 0;

        }

        public async Task<IEnumerable<UserMenuPermissionDto>> GetAllAsync(int Skip = 0, int Limit = 10, string? Search = null, CancellationToken cancellationToken = default)
        {
            var data = await _cRDBContext.UserMenuPermissions.AsNoTracking()
                .Where(x => x.UserId.ToString().Contains(Search) || x.MenuId.ToString().Contains(Search) || Search == null)
                .Skip(Skip)
                .Take(Limit)
                .ToListAsync(cancellationToken);
            return data.Adapt<IEnumerable<UserMenuPermissionDto>>();
        }

        public Task<int> GetAllCountAsync(string? Search = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<UserMenuPermissionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var data = await _cRDBContext.UserMenuPermissions.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            return data?.Adapt<UserMenuPermissionDto>();
        }

        public async Task<List<MenuDto>> GetMenusByUserIdAsync(int userId, CancellationToken cancellationToken =default)
        {
            var menus = await _cRDBContext.UserMenuPermissions
                       .Where(x => x.UserId == userId)
                       .Where(x => x.IsActive == true)
                       .Where(x => x.IsDeleted == false)
                       .Select(x => x.Menu.Adapt<MenuDto>())  
                       .AsNoTracking()
                       .ToListAsync();
            return menus;
        }

        public async Task<UserMenuPermissionDto> UpdateAsync(UserMenuPermissionDto entity, int Id, int UserId, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingData = await _cRDBContext.UserMenuPermissions.FirstOrDefaultAsync(x => x.Id == Id, cancellationToken);
                if (existingData == null)
                    throw new Exception("User menu permission not found");
                existingData.UserId = entity.UserId;
                existingData.MenuId = entity.MenuId;
                existingData.IsActive = entity.IsActive;
                existingData.UpdatedAt = DateTime.UtcNow;
                existingData.UpdatedBy = UserId;
                var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
                if (result > 0)
                {
                    return existingData.Adapt<UserMenuPermissionDto>();
                }
                throw new Exception("Failed to update user menu permission");
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Database update error: " + (ex.InnerException?.Message ?? ex.Message), ex);

            }
        }
    }
}
