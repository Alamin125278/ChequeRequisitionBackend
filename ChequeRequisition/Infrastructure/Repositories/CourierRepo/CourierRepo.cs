using ChequeRequisiontService.Core.Dto.Branch;
using ChequeRequisiontService.Core.Dto.Courier;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.DbContexts;
using DocumentFormat.OpenXml.Math;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ChequeRequisiontService.Infrastructure.Repositories.CourierRepo
{
    public class CourierRepo(CRDBContext cRDBContext) : ICourierRepo
    {
        private CRDBContext _cRDBContext = cRDBContext;
        public Task<CourierDto> CreateAsync(CourierDto entity, int UserId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id, int UserId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CourierDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var data = await _cRDBContext.Couriers.AsNoTracking()
               .Where(x => (x.IsDelete == false) &&( x.IsActive==true))
               .ToListAsync(cancellationToken);

            return data.Adapt<IEnumerable<CourierDto>>();
        }

        public Task<IEnumerable<CourierDto>> GetAllAsync(int Skip = 0, int Limit = 10, string? Search = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetAllCountAsync(string? Search = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<CourierDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<CourierDto> UpdateAsync(CourierDto entity, int Id, int UserId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
