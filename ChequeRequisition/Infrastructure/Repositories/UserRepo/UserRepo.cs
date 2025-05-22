using ChequeRequisiontService.Core.Dto.User;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.DbContexts;
using ChequeRequisiontService.Models.CRDB;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ChequeRequisiontService.Infrastructure.Repositories.UserRepo;

public class UserRepo(CRDBContext cRDBContext) : IUserRepo
{
    private readonly CRDBContext _cRDBContext = cRDBContext;
    public async Task<UserDto> CreateAsync(UserDto entity, int UserId, CancellationToken cancellationToken = default)
    {
        try
        {
            var data = entity.Adapt<User>();
            data.CreatedAt = DateTime.UtcNow;
            data.CreatedBy = UserId;

            await _cRDBContext.Users.AddAsync(data, cancellationToken);
            var result = await _cRDBContext.SaveChangesAsync(cancellationToken);

            if (result > 0)
            {
                return data.Adapt<UserDto>();
            }

            throw new Exception("Failed to create user");
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("Database update error: " + (ex.InnerException?.Message ?? ex.Message), ex);
        }
    }


    public async Task<bool> DeleteAsync(int id, int UserId, CancellationToken cancellationToken = default)
    {
       var user= await _cRDBContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return false;
    
            user.IsDelete = true;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = UserId;
    
            var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
            return result > 0;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync(int Skip = 0, int Limit = 10, string? Search = null, CancellationToken cancellationToken = default)
    {
        var data = await _cRDBContext.Users.AsNoTracking()
            .Where(x => x.UserName.Contains(Search) || x.Email.Contains(Search) || Search==null)
            .Skip(Skip)
            .Take(Limit)
            .ToListAsync();
        return data.Adapt<IEnumerable<UserDto>>();
    }

    public Task<IEnumerable<UserDto>> GetAllAsync(int RoleId, int Skip = 0, int Limit = 10, string? Search = null)
    {
        throw new NotImplementedException();
    }

    public async Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var data = await _cRDBContext.Users.FirstOrDefaultAsync(x => x.Id == id);

        return data?.Adapt<UserDto?>();
    }

    public async Task<UserDto> UpdateAsync(UserDto entity, int Id, int UserId, CancellationToken cancellationToken = default)
    {
        var data = await _cRDBContext.Users.FirstOrDefaultAsync(x => x.Id == Id);
        if (data == null)
        {
            throw new NotFoundException("User not found");
        }
        data.Name = entity.Name;
        data.UserName = entity.UserName;
        data.Email = entity.Email;
        data.PasswordHash = entity.PasswordHash;
        data.Role = entity.Role;
        data.BankId = entity.BankId;
        data.BranchId = entity.BranchId;
        data.VendorId = entity.VendorId;
        data.ImagePath = entity.ImagePath;
        data.IsActive = entity.IsActive;

        data.UpdatedAt = DateTime.UtcNow;
        data.UpdatedBy = UserId;

        try
        {
            var result = await _cRDBContext.SaveChangesAsync(cancellationToken);

            if (result > 0)
            {
                return data.Adapt<UserDto>();
            }

            throw new Exception("Failed to create user");
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("Database update error", ex.InnerException ?? ex);
        }
    }
}
