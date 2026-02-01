using ChequeRequisiontService.Core.Dto.Menu;

namespace ChequeRequisiontService.Core.Interfaces.Repositories
{
    public interface IMenuRepo:IGenericRepository<MenuDto>
    {
        Task<int> GetAllCountAsync(string? Search = null, bool? IsActive = null, CancellationToken cancellationToken = default);
    }
}
