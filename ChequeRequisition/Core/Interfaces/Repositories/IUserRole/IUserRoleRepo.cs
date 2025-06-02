using ChequeRequisiontService.Core.Dto.UserRole;

namespace ChequeRequisiontService.Core.Interfaces.Repositories.IUserRole;

public interface IUserRoleRepo:IGenericRepository<UserRoleDto>
{
    public Task<IEnumerable<UserRoleDto>> GetAllAsync(int? bankId, CancellationToken cancellationToken = default);
}
