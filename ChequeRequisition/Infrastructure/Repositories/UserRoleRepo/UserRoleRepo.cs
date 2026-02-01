using ChequeRequisiontService.Core.Dto.UserRole;
using ChequeRequisiontService.Core.Interfaces.Repositories.IUserRole;
using ChequeRequisiontService.DbContexts;
using ChequeRequisiontService.Models.CRDB;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Wordprocessing;
using Mapster;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ChequeRequisiontService.Infrastructure.Repositories.UserRoleRepo;

public class UserRoleRepo(CRDBContext cRDBContext) : IUserRoleRepo
{
    private readonly CRDBContext _cRDBContext = cRDBContext;
    public async Task<UserRoleDto> CreateAsync(UserRoleDto entity, int UserId, CancellationToken cancellationToken = default)
    {
        try
        {
            var data = entity.Adapt<UserRole>();
            data.CreatedAt = DateTime.UtcNow;
            data.CreatedBy = UserId;
            data.IsDeleted = false;
            await _cRDBContext.UserRoles.AddAsync(data, cancellationToken);
            var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
            if (result > 0)
            {
                return data.Adapt<UserRoleDto>();
            }
            throw new Exception("Failed to create user role");
        }
        catch(DbUpdateException ex)
        {
            throw new Exception("Database update error: " + (ex.InnerException?.Message ?? ex.Message), ex);
        }
    }

    public async Task<bool> DeleteAsync(int id, int UserId, CancellationToken cancellationToken = default)
    {
        var userRole = await _cRDBContext.UserRoles
            .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false, cancellationToken);
        if (userRole == null)
            return false;
        userRole.IsDeleted = true;
        userRole.UpdatedAt = DateTime.UtcNow;
        userRole.UpdatedBy = UserId;
        var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
        return result > 0;
    }

    public async Task<IEnumerable<UserRoleDto>> GetAllAsync(int Skip = 0, int Limit = 10, string? Search = null, CancellationToken cancellationToken = default)
    {
        var data = await _cRDBContext.UserRoles.AsNoTracking()
            .Where(x => (x.RoleName.Contains(Search) || Search == null))
            .Where(x => x.IsDeleted == false)
            .Skip(Skip)
            .Take(Limit)
            .ToListAsync(cancellationToken);
        return data.Adapt<IEnumerable<UserRoleDto>>();
    }

    public async Task<IEnumerable<UserRoleDto>> GetAllAsync(int? bankId, CancellationToken cancellationToken = default)
    {
        var query = _cRDBContext.UserRoles.AsNoTracking()
        .Where(x => x.IsDeleted == false)
        .Where(x=> x.IsActive == true);

        if (bankId != null)
        {
            query = query.Where(x => x.Id != 1 && x.Id != 2);
        }
        var data =await query.ToListAsync(cancellationToken);
        return data.Adapt<IEnumerable<UserRoleDto>>();
    }

    public async Task<IEnumerable<UserRoleDto>> GetAllAsync(int Skip = 0, int Limit = 10, string? Search = null, bool? IsActive = null, CancellationToken cancellationToken = default)
    {
        IQueryable<UserRole> query = _cRDBContext.UserRoles.AsNoTracking();

        // Base filter
        query = query.Where(x => x.IsDeleted==false);

        // Filter: active/inactive
        if (IsActive.HasValue)
            query = query.Where(x => x.IsActive == IsActive.Value);

        // Filter: search by role name
        if (!string.IsNullOrWhiteSpace(Search))
            query = query.Where(x => EF.Functions.Like(x.RoleName, $"%{Search}%"));

        // Pagination
        query = query.Skip(Skip).Take(Limit);

        var data = await query.ToListAsync(cancellationToken);

        return data.Adapt<IEnumerable<UserRoleDto>>();
    }

    public Task<int> GetAllCountAsync(string? Search = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<UserRoleDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var userRole = await _cRDBContext.UserRoles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false, cancellationToken);
        return userRole?.Adapt<UserRoleDto>();
    }

    public async Task<UserRoleDto> UpdateAsync(UserRoleDto entity, int Id, int UserId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userRole = await _cRDBContext.UserRoles.FirstOrDefaultAsync(x => x.Id == Id && x.IsDeleted == false) ?? throw new Exception("User role not found");
            userRole.RoleName = entity.RoleName;
            userRole.IsActive = entity.IsActive;
            userRole.UpdatedAt = DateTime.UtcNow;
            userRole.UpdatedBy = UserId;
            var result =await _cRDBContext.SaveChangesAsync(cancellationToken);
            if (result > 0)
            {
              return userRole.Adapt<UserRoleDto>();
            }
            throw new Exception("Failed to update user role");
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("Database update error: " + (ex.InnerException?.Message ?? ex.Message), ex);
        }

    }
}
