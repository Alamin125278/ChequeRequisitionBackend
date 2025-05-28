using ChequeRequisiontService.Core.Dto.User;
using ChequeRequisiontService.Core.Dto.Vendor;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.DbContexts;
using ChequeRequisiontService.Models.CRDB;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace ChequeRequisiontService.Infrastructure.Repositories.VendorRepo
{
    public class VendorRepo(CRDBContext cRDBContext) : IVendorRepo
    {
        private readonly CRDBContext _cRDBContext = cRDBContext;
        public async Task<VendorDto> CreateAsync(VendorDto entity, int UserId, CancellationToken cancellationToken = default)
        {
            try
            {
                var data = entity.Adapt<Vendor>();
                data.CreatedAt = DateTime.UtcNow;

                await _cRDBContext.Vendors.AddAsync(data, cancellationToken);
                var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
                if (result > 0)
                {
                    return data.Adapt<VendorDto>();
                }
                throw new Exception("Failed to create vendor");

            }
            catch(DbUpdateException ex)
            {
                throw new Exception("Database update error: " + (ex.InnerException?.Message ?? ex.Message), ex);
            }
        }

        public async Task<bool> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default)
        {
            var vendor = await _cRDBContext.Vendors
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (vendor == null)
                return false;

            vendor.IsDelete = true;
            vendor.UpdatedAt = DateTime.UtcNow;
            // Optional: Track who performed the delete
             //vendor.UpdatedBy = userId;

            var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }


        public async Task<IEnumerable<VendorDto>> GetAllAsync(int Skip = 0, int Limit = 10, string? Search = null, CancellationToken cancellationToken = default)
        {
            var data = await _cRDBContext.Vendors.AsNoTracking()
                .Where(x => x.VendorName.Contains(Search) || x.Email.Contains(Search) ||x.Phone.Contains(Search) || Search == null)
                .Where(x => x.IsDelete == false)
                .Skip(Skip)
                .Take(Limit)
                .ToListAsync(cancellationToken);
            return data.Adapt<IEnumerable<VendorDto>>();
        }

        public Task<int> GetAllCountAsync(string? Search = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<VendorDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var data =await _cRDBContext.Vendors.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            return data?.Adapt<VendorDto?>();
        }

        public async Task<VendorDto> UpdateAsync(VendorDto entity, int Id, int UserId, CancellationToken cancellationToken = default)
        {
           var data = await _cRDBContext.Vendors.FirstOrDefaultAsync(x => x.Id == Id, cancellationToken);
            if (data != null)
            {
                data.VendorName = entity.VendorName;
                data.Email = entity.Email;
                data.Phone = entity.Phone;
                data.Address = entity.Address;
                data.PhotoPath = entity.PhotoPath;
                data.UpdatedAt = DateTime.UtcNow;
                var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
                if (result > 0)
                {
                    return data.Adapt<VendorDto>();
                }
                throw new Exception("Failed to update vendor");
            }
            throw new Exception("Vendor not found");
        }
    }
}
