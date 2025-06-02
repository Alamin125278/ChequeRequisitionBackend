using ChequeRequisiontService.Core.Dto.DefaultMenuPermission;

namespace ChequeRequisiontService.Core.Interfaces.Repositories;

public interface IDefaultMenuPermisionRepo:IGenericRepository<DefaultMenuPermisionDto>
{
    Task<IEnumerable<DefaultMenuPermisionDto>> GetAllAsync(int Role, CancellationToken cancellationToken = default);
}
