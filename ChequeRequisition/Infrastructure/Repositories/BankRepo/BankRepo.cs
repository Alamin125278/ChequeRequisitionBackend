using ChequeRequisiontService.Core.Dto.Bank;
using ChequeRequisiontService.Core.Dto.Vendor;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.DbContexts;
using ChequeRequisiontService.Models.CRDB;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;

namespace ChequeRequisiontService.Infrastructure.Repositories.BankRepo
{
    public class BankRepo(CRDBContext cRDBContext) : IBankRepo

    {
        private readonly CRDBContext _cRDBContext = cRDBContext;
        public async Task<BankDto> CreateAsync(BankDto entity, int UserId, CancellationToken cancellationToken = default)
        {
            try 
            {
                var data = entity.Adapt<Bank>();
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy= UserId;
                data.IsDeleted = false;
               await _cRDBContext.Banks.AddAsync(data);
                var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
                if (result > 0)
                {
                    return data.Adapt<BankDto>();   
                }
                throw new Exception("Failed to create bank");
            }
            catch(DbUpdateException ex)
            {
                throw new Exception("Database update error: " + (ex.InnerException?.Message ?? ex.Message), ex);
            }
        }

        public async Task<bool> DeleteAsync(int id, int UserId, CancellationToken cancellationToken = default)
        {
            var bank =await _cRDBContext.Banks
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if(bank == null)
                return false;
            bank.IsDeleted = true;
            bank.UpdatedAt = DateTime.UtcNow;
            bank.UpdatedBy = UserId;
            var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }

        public async Task<IEnumerable<BankDto>> GetAllAsync(int Skip = 0, int Limit = 10, string? Search = null, CancellationToken cancellationToken = default)
        {
            var data = await _cRDBContext.Banks.AsNoTracking()
                .Where(x => x.BankName.Contains(Search) ||x.BankEmail.Contains(Search) || x.BankCode.Contains(Search) || x.RoutingNumber.Contains(Search) || Search == null)
                .Where(x => x.IsDeleted == false)
                .Skip(Skip)
                .Take(Limit)
                .ToListAsync(cancellationToken);
            return data.Adapt<IEnumerable<BankDto>>();
        }

        public async Task<IEnumerable<BankDto>> GetAllAsync(int Skip = 0, int Limit = 10, string? Search = null, bool? IsActive = null, CancellationToken cancellationToken = default)
        {
            
            var query = _cRDBContext.Banks.AsNoTracking()
                .Include(x => x.Vendor)
                .Where(x => (x.BankName.Contains(Search) || x.BankEmail.Contains(Search) || x.BankCode.Contains(Search) || x.RoutingNumber.Contains(Search) || Search == null))
                .Where(x => x.IsDeleted == false)
                .Where(x=>x.IsActive==IsActive || IsActive==null);

            var data = await query
                .Skip(Skip)
                .Take(Limit)
                .ToListAsync(cancellationToken);
            return data.Adapt<IEnumerable<BankDto>>();
        }

        public async Task<IEnumerable<BankDto>> GetAllAsync(int? BankId, CancellationToken cancellationToken)
        {
            var query = _cRDBContext.Banks.AsNoTracking()
                 .Where(x=> x.Id==BankId || BankId==null)
                 .Where(x => x.IsDeleted == false)
                 .Where(x => x.IsActive == true);

            var data = await query
                .ToListAsync(cancellationToken);
            return data.Adapt<IEnumerable<BankDto>>();
        }

        public async Task<int> GetAllCountAsync(string? Search = null, bool? IsActive = null, CancellationToken cancellationToken = default)
        {
            var count = await _cRDBContext.Banks.AsNoTracking()
                .Where(x => (x.BankName.Contains(Search) || x.BankEmail.Contains(Search) || x.BankCode.Contains(Search) || x.RoutingNumber.Contains(Search) || Search == null))
                .Where(x => (x.IsDeleted == false))
                .Where(x=>x.IsActive==IsActive || IsActive==null)
                .Select(x=>x.Id)
                .CountAsync();
            return count;
        }

        public async Task<int> GetAllCountAsync(string? Search = null, CancellationToken cancellationToken = default)
        {
            var count =await  _cRDBContext.Banks.AsNoTracking()
                .Where(x => (x.BankName.Contains(Search) || x.BankEmail.Contains(Search) || x.BankCode.Contains(Search) || x.RoutingNumber.Contains(Search) || Search == null))
                .Where(x => (x.IsDeleted == false))
                .Select(x => x.Id)
                .CountAsync();
            return count;
        }

        public async Task<BankDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var data = await _cRDBContext.Banks.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            return data?.Adapt<BankDto?>();
        }

        public async Task<BankDto> UpdateAsync(BankDto entity, int id, int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var data = await _cRDBContext.Banks
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                if (data == null)
                    return null;
                data = entity.Adapt(data);

                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = userId;

                var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
                if (result > 0)
                {
                    return data.Adapt<BankDto>();
                }

                throw new Exception("Failed to update bank");
            }
            catch(DbUpdateException ex)
            {
                throw new Exception("Database update error: " + (ex.InnerException?.Message ?? ex.Message), ex);
            }

        }

    }
}
