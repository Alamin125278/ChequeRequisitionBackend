using ChequeRequisiontService.Core.Dto.DefaultMenuPermission;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.DbContexts;
using ChequeRequisiontService.Models.CRDB;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace ChequeRequisiontService.Infrastructure.Repositories.DefaultMenuPermision
{
    public class DefaultMenuPermisionRepo(CRDBContext cRDBContext) : IDefaultMenuPermisionRepo
    {

        private readonly CRDBContext _cRDBContext = cRDBContext;
        public async Task<DefaultMenuPermisionDto> CreateAsync(DefaultMenuPermisionDto entity, int UserId, CancellationToken cancellationToken = default)
        {
            try
            {
                var data = entity.Adapt<UserRoleDefaultMenuPermission>();
                data.IsDeleted = false;
                data.CreatedBy = UserId;
                data.CreatedAt = DateTime.UtcNow;
                await _cRDBContext.UserRoleDefaultMenuPermissions.AddAsync(data, cancellationToken);
                var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
                if (result > 0)
                {
                    return data.Adapt<DefaultMenuPermisionDto>();
                }
                throw new Exception("Failed to create DefaultMenuPermision");
            }
            catch(DbException ex)
            {
                // Log the exception (ex) here
                throw new Exception("Database error occurred while creating DefaultMenuPermision.", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id, int UserId, CancellationToken cancellationToken = default)
        {
            var defaultMenuPermision = await _cRDBContext.UserRoleDefaultMenuPermissions
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false, cancellationToken);
            if (defaultMenuPermision == null)
                return false;
            defaultMenuPermision.IsDeleted = true;
            defaultMenuPermision.UpdatedAt = DateTime.UtcNow;
            defaultMenuPermision.UpdatedBy = UserId;
            var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }

        public async Task<IEnumerable<DefaultMenuPermisionDto>> GetAllAsync(int Skip = 0, int Limit = 10, string? Search = null, CancellationToken cancellationToken = default)
        {
            var data = await _cRDBContext.UserRoleDefaultMenuPermissions.AsNoTracking()
                .Where(x => (x.MenuId.ToString().Contains(Search) || x.RoleId.ToString().Contains(Search) || Search == null) && x.IsDeleted == false)
                .Skip(Skip)
                .Take(Limit)
                .ToListAsync(cancellationToken);
           if(data == null)
                return [];
            return data.Adapt<IEnumerable<DefaultMenuPermisionDto>>();
        }

        public Task<int> GetAllCountAsync(string? Search = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<DefaultMenuPermisionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var defaultMenuPermision = await _cRDBContext.UserRoleDefaultMenuPermissions
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false, cancellationToken);
            if (defaultMenuPermision == null)
                return null;
            return defaultMenuPermision.Adapt<DefaultMenuPermisionDto>();
        }

        public async Task<DefaultMenuPermisionDto> UpdateAsync(DefaultMenuPermisionDto entity, int Id, int UserId, CancellationToken cancellationToken = default)
        {
            try
            {
                var defaultMenuPermision = await _cRDBContext.UserRoleDefaultMenuPermissions
                    .FirstOrDefaultAsync(x => x.Id == Id && x.IsDeleted == false, cancellationToken);
                if (defaultMenuPermision == null)
                    throw new Exception("DefaultMenuPermision not found");
                var updatedData = entity.Adapt(defaultMenuPermision);
                updatedData.UpdatedAt = DateTime.UtcNow;
                updatedData.UpdatedBy = UserId;
                var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
                if (result > 0)
                {
                    return updatedData.Adapt<DefaultMenuPermisionDto>();
                }
                throw new Exception("Failed to update DefaultMenuPermision");
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Database update error: " + (ex.InnerException?.Message ?? ex.Message), ex);
            }
        }
    }
}
