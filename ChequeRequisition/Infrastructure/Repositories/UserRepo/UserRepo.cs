using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.User;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.DbContexts;
using ChequeRequisiontService.Models.CRDB;
using DocumentFormat.OpenXml.Spreadsheet;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;

namespace ChequeRequisiontService.Infrastructure.Repositories.UserRepo;

public class UserRepo(CRDBContext cRDBContext) : IUserRepo
{
    private readonly CRDBContext _cRDBContext = cRDBContext;

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return _cRDBContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task<UserDto> CreateAsync(UserDto entity, int UserId, CancellationToken cancellationToken = default)
    {
        try
        {
            var data = entity.Adapt<User>();
            data.CreatedAt = DateTime.UtcNow;
            data.CreatedBy = UserId;
            data.IsDelete = false;
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
       var user= await _cRDBContext.Users.FirstOrDefaultAsync(x => x.Id == id,cancellationToken);
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


    public async Task<IEnumerable<UserDto>> GetAllAsync(
     int? BankId = null,
     int? BranchId = null,
     int? RoleId = null,
     int Skip = 0,
     int Limit = 10,
     string? Search = null,
     bool? IsActive = null,
     CancellationToken cancellationToken = default)
    {
        var query = _cRDBContext.Users
            .AsNoTracking()
            .Include(x => x.BankCreatedByNavigations)
            .Include(x => x.BankUpdatedByNavigations)
            .Include(x => x.RoleNavigation)
            .Include(x => x.Vendor)
            .Include(x => x.BranchUpdatedByNavigations)
            .Include(x => x.BranchCreatedByNavigations)
            .Where(x => x.UserName.Contains(Search) || x.Email.Contains(Search) || x.Name.Contains(Search) || Search == null)
               .Where(x => x.IsDelete== false)
               .Where(x => x.IsActive == IsActive || IsActive == null)
               .Where(x => x.BankId == BankId || BankId == null)
               .Where(x => x.BranchId == BranchId || BranchId == null)
               .Where(x => x.Role == RoleId || RoleId == null)
            .OrderBy(x => x.Id)
            .Skip(Skip)
            .Take(Limit);

        var users = await query.ToListAsync(cancellationToken);
        return users.Adapt<IEnumerable<UserDto>>();
    }


    public Task<int> GetAllCountAsync(string? Search = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<int> GetAllCountAsync(int? BankId = null, int? BranchId = null, int? RoleId = null, string? Search = null, bool? IsActive = null, CancellationToken cancellationToken = default)
    {
        var query = _cRDBContext.Users.AsNoTracking()
            .Where(x => x.UserName.Contains(Search) || x.Email.Contains(Search) || x.Name.Contains(Search) || Search == null)
               .Where(x => x.IsDelete == false)
               .Where(x => x.IsActive == IsActive || IsActive == null)
               .Where(x => x.BankId == BankId || BankId == null)
               .Where(x => x.BranchId == BranchId || BranchId == null)
               .Where(x => x.Role == RoleId || RoleId == null);
        var count = await query.CountAsync(cancellationToken);
        return count;
    }

    public async Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var data = await _cRDBContext.Users
       .Include(x => x.BankCreatedByNavigations)
.Include(x => x.BankUpdatedByNavigations)
       .Include(x => x.RoleNavigation)
       .Include(x => x.Vendor)
       .Include(x => x.BankCreatedByNavigations)
.Include(x => x.BankUpdatedByNavigations)
       .FirstOrDefaultAsync(x => x.Id == id);


        return data?.Adapt<UserDto?>();
    }

    public async Task<UserDto?> GetUserByEmailOrUserName(string UserNameOrEmail)
    {
        var data = await _cRDBContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserName == UserNameOrEmail || x.Email == UserNameOrEmail);
        return data?.Adapt<UserDto?>();
    }

    public async Task<UserDto> UpdateAsync(UserDto entity, int Id, int UserId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _cRDBContext.Users.FirstOrDefaultAsync(x => x.Id == Id) ?? throw new NotFoundException("User not found");
            var existingPasswordHash = user.PasswordHash;
            user.Name = entity.Name ?? user.Name;
            user.Email = entity.Email ?? user.Email;
            user.UserName = entity.UserName ?? user.UserName;
            user.ImagePath = entity.ImagePath ?? user.ImagePath;
            user.Role = entity.Role ?? user.Role;
            user.IsActive = entity.IsActive ?? user.IsActive;
            user.BankId = entity.BankId ?? user.BankId;
            user.BranchId = entity.BranchId ?? user.BranchId;
            user.VendorId = entity.VendorId ?? user.VendorId;

            user.UpdatedAt = DateTime.UtcNow;
            user.PasswordHash = existingPasswordHash;
            user.UpdatedBy = UserId;
            var result = await _cRDBContext.SaveChangesAsync(cancellationToken);

            if (result > 0)
            {
                return user.Adapt<UserDto>();
            }

            throw new Exception("Failed to update user");
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("Database update error", ex.InnerException ?? ex);
        }
    }

    public async Task<bool> UpdatedPasswordAsunc(int Id, ChangedPasswordDto entity, int userId, CancellationToken cancellationToken = default)
    {
        var user = await _cRDBContext.Users.FirstOrDefaultAsync(x => x.Id == Id);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }
        user.PasswordHash = entity.NewPassword;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = userId;
        var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
        if (result > 0)
        {
            return true;
        }
        throw new Exception("Failed to update password");
    }
}
