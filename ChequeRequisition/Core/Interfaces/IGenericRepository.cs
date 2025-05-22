namespace ChequeRequisiontService.Core.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(int Skip = 0, int Limit = 10, string? Search = null, CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<T> CreateAsync(T entity, int UserId, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, int Id, int UserId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int UserId, CancellationToken cancellationToken = default);
}
