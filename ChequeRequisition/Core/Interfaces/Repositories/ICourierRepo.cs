using ChequeRequisiontService.Core.Dto.Courier;

namespace ChequeRequisiontService.Core.Interfaces.Repositories
{
    public interface ICourierRepo:IGenericRepository<CourierDto>
    {
        Task<IEnumerable<CourierDto>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}

