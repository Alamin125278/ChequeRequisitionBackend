using ChequeRequisiontService.Core.Dto.Bank;
using ChequeRequisiontService.Core.Dto.Branch;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.DbContexts;
using ChequeRequisiontService.Models.CRDB;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ChequeRequisiontService.Infrastructure.Repositories.BranchRepo
{
    public class BranchRepo(CRDBContext cRDBContext) : IBranchRepo
    {
        private CRDBContext _cRDBContext = cRDBContext;
        public async Task<BranchDto> CreateAsync(BranchDto entity, int UserId, CancellationToken cancellationToken = default)
        {
            try
            {
                var data = entity.Adapt<Branch>();
                data.CreatedAt = DateTime.UtcNow;
                data.CreatedBy = UserId;
                data.IsDeleted = false;
                await _cRDBContext.Branches.AddAsync(data, cancellationToken);
                var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
                if (result > 0)
                {
                    return data.Adapt<BranchDto>();
                }
                throw new Exception("Failed to create branch");
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Database update error: " + (ex.InnerException?.Message ?? ex.Message), ex);
            }
        }

        public async Task<bool> DeleteAsync(int id, int UserId, CancellationToken cancellationToken = default)
        {
            var branch = await _cRDBContext.Branches
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (branch == null)
               return false;
            branch.IsDeleted = true;
            branch.UpdatedAt = DateTime.UtcNow;
            branch.UpdatedBy = UserId;
            var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
            return result > 0;
        }

        public async Task<IEnumerable<BranchDto>> GetAllAsync(int Skip = 0, int Limit = 10, string? Search = null, CancellationToken cancellationToken = default)
        {
            var data = await _cRDBContext.Branches.AsNoTracking()
                .Where(x => x.BranchName.Contains(Search) || x.BranchEmail.Contains(Search) || x.BranchCode.Contains(Search) || x.RoutingNo.Contains(Search) || Search == null)
                .Where(x => x.IsDeleted == false)
                .Skip(Skip)
                .Take(Limit)
                .ToListAsync(cancellationToken);

            return data.Adapt<IEnumerable<BranchDto>>();
        }

     

        public async Task<IEnumerable<BranchDto>> GetAllAsync(int? BankId = null, int Skip = 0, int Limit = 10, string? Search = null, bool? IsActive = null, CancellationToken cancellationToken = default)
        {
            var query = _cRDBContext.Branches.AsNoTracking()
               .Include(x => x.Bank)
               .Where(x => x.BranchName.Contains(Search) || x.BranchEmail.Contains(Search) || x.BranchCode.Contains(Search) || x.RoutingNo.Contains(Search) || Search == null)
               .Where(x => x.IsDeleted == false)
               .Where(x => x.IsActive == IsActive || IsActive == null)
               .Where(x => x.BankId == BankId || BankId == null);

            var data = await query
                .Skip(Skip)
                .Take(Limit)
                .ToListAsync(cancellationToken);
            return data.Adapt<IEnumerable<BranchDto>>();
        }

        public async Task<IEnumerable<BranchDto>> GetAllAsync(int? BankId = null, CancellationToken cancellationToken = default)
        {
            var query = _cRDBContext.Branches.AsNoTracking()
               .Where(x => x.IsDeleted == false)
               .Where(x => x.IsActive == true)
               .Where(x => x.BankId == BankId || BankId == null);

            var data = await query.ToListAsync(cancellationToken);
            return data.Adapt<IEnumerable<BranchDto>>();
        }
        public Task<int> GetAllCountAsync(string? Search = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetAllCountAsync(string? Search = null, int? BankId = null, bool? IsActive = null, CancellationToken cancellationToken = default)
        {
            var count = await _cRDBContext.Branches.AsNoTracking()
                .Where(x => (x.BranchName.Contains(Search) || x.BranchEmail.Contains(Search) || x.BranchCode.Contains(Search) || x.RoutingNo.Contains(Search) || Search == null))
                .Where(x => x.IsDeleted == false)
                .Where(x => x.IsActive == IsActive || IsActive == null)
                .Where(x => x.BankId == BankId || BankId == null)
                .CountAsync(cancellationToken);
            return count;

        }

        public async Task<BranchDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var data = await _cRDBContext.Branches.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false, cancellationToken);
            return data?.Adapt<BranchDto>();
        }

        public async Task<BranchDto> UpdateAsync(BranchDto entity, int Id, int UserId, CancellationToken cancellationToken = default)
        {
            try
            {
                var data = await _cRDBContext.Branches
                .FirstOrDefaultAsync(x => x.Id == Id && x.IsDeleted == false, cancellationToken);
                if (data == null)
                    return null;
                data = entity.Adapt(data);
                data.UpdatedAt = DateTime.UtcNow;
                data.UpdatedBy = UserId;

                var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
                if (result > 0)
                {
                    return data.Adapt<BranchDto>();
                }
                throw new Exception("Failed to update branch");
            }
            catch(DbUpdateException ex)
            {
                throw new Exception("Database update error: " + (ex.InnerException?.Message ?? ex.Message), ex);
            }
            
            
        }
    }
}
