using ChequeRequisiontService.Core.Dto.User;
using Microsoft.EntityFrameworkCore.Storage;

namespace ChequeRequisiontService.Core.Interfaces.Repositories;

public interface IUserRepo : IGenericRepository<UserDto> 
{
    Task<IEnumerable<UserDto>> GetAllAsync(int? BankId=null, int? BranchId = null, int? RoleId=null, int Skip = 0, int Limit = 10, string? Search = null,bool?IsActive=null, CancellationToken cancellationToken=default);
    Task<UserDto?> GetUserByEmailOrUserName(string UserNameOrEmail);
    Task<int> GetAllCountAsync(int? BankId = null, int? BranchId = null, int? RoleId = null, string? Search = null, bool? IsActive = null, CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

}

