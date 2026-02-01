using ChequeRequisiontService.Core.Dto.Branch;
using ChequeRequisiontService.Core.Dto.UserRole;

namespace ChequeRequisiontService.Core.Interfaces.Repositories.IUserRole;

public interface IUserRoleRepo:IGenericRepository<UserRoleDto>
{
    Task<IEnumerable<UserRoleDto>> GetAllAsync(int Skip = 0, int Limit = 10, string? Search = null, bool? IsActive = null, CancellationToken cancellationToken = default);
    public Task<IEnumerable<UserRoleDto>> GetAllAsync(int? bankId, CancellationToken cancellationToken = default);
}
