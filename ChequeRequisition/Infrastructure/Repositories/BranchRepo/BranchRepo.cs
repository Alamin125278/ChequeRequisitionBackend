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
                .OrderByDescending(x => x.Id)
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
                .OrderByDescending(x => x.Id)
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

            var data = await query.OrderByDescending(x => x.Id).ToListAsync(cancellationToken);
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

        public async Task<BranchDto?> GetIdAsync(
    int bankId,
    string? branchName = null,
    string? branchCode = null,
    string? IsAgent = null,
    CancellationToken cancellationToken = default)
        {
            // Base query
            var query = _cRDBContext.Branches
                .AsNoTracking()
                .Where(x => x.BankId == bankId
         && x.IsDeleted == false
         && x.IsActive == true);

            string? trimmedBranchName = branchName;

            // Handle branchName cleanup
            if (!string.IsNullOrEmpty(branchName))
            {
                if (IsAgent == "Agent")
                {
                    int dashIndex = branchName.IndexOf('-');

                    if (dashIndex >= 0)
                    {
                        var afterDash = branchName[(dashIndex + 1)..].Trim();

                        int bracketIndex = afterDash.IndexOf('(');
                        trimmedBranchName = bracketIndex > 0
                            ? afterDash[..bracketIndex].Trim()
                            : afterDash;
                    }
                }
                else if (branchName.Contains(','))
                {
                    trimmedBranchName = branchName[..branchName.IndexOf(',')].Trim();
                }
            }

            // Apply filters
            if (!string.IsNullOrEmpty(branchCode))
            {
                if (branchCode == "PO")
                {
                    query = query.Where(x =>
                        x.BranchName != null &&
                        x.BranchName.StartsWith(trimmedBranchName));
                }
                else if (IsAgent == "Agent")
                {
                    query = query.Where(x =>
                        x.BranchCode == branchCode &&
                        x.BranchName!.StartsWith(trimmedBranchName));
                }
                else if (string.IsNullOrEmpty(branchName))
                {
                    query = query.Where(x => x.BranchCode == branchCode);
                }
                else
                {
                    query = query.Where(x =>
                        x.BranchCode == branchCode &&
                        x.BranchName!.StartsWith(trimmedBranchName));
                }
            }
            else
            {
                query = query.Where(x => x.BranchName == branchName);
            }

            var data = await query.FirstOrDefaultAsync(cancellationToken);

            return data?.Adapt<BranchDto>();
        }



        public async Task<BranchDto?> UpdateAsync(BranchDto updatedDto, int id, int userId, CancellationToken cancellationToken = default)
        {
            var existingBranch = await _cRDBContext.Branches
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

            if (existingBranch is null)
                return null;

            // Map updated data to a new object with existing ID
            var updatedBranch = updatedDto.Adapt<Branch>();
            updatedBranch.Id = id;
            updatedBranch.UpdatedAt = DateTime.UtcNow;
            updatedBranch.UpdatedBy = userId;

            // Attach and mark as modified
            _cRDBContext.Branches.Attach(updatedBranch);
            _cRDBContext.Entry(updatedBranch).State = EntityState.Modified;

            try
            {
                var affectedRows = await _cRDBContext.SaveChangesAsync(cancellationToken);
                return affectedRows > 0 ? updatedBranch.Adapt<BranchDto>() : null;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Database update error: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }

    }
}
