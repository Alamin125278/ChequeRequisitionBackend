using ChequeRequisiontService.Core.Dto.Branch;
using ChequeRequisiontService.Core.Dto.LocalFileImportLog;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.DbContexts;
using ChequeRequisiontService.Models.CRDB;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ChequeRequisiontService.Infrastructure.Repositories.LocalFileImportRepo
{
    public class LocalFileImportRepo(CRDBContext cRDBContext) : ILocalFileImportLogRepo
    {
        private readonly CRDBContext _cRDBContext = cRDBContext;
        public async Task<LocalFileImportLogDto> CreateAsync(LocalFileImportLogDto entity, int UserId, CancellationToken cancellationToken = default)
        {
            try
            {
                var data = entity.Adapt<LocalFileImport>();
                data.CreatedAt = DateTime.UtcNow;
                data.ImportedAt = DateOnly.FromDateTime(DateTime.UtcNow);
                data.CreatedBy = UserId;
                await _cRDBContext.LocalFileImports.AddAsync(data, cancellationToken);
                var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
                if (result > 0)
                {
                    return data.Adapt<LocalFileImportLogDto>();
                }
                throw new Exception("Failed to Import Log File");
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Database update error: " + (ex.InnerException?.Message ?? ex.Message), ex);
            }
        }

        public Task<bool> DeleteAsync(int id, int UserId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ExistsAsync(int bankId, string filename, CancellationToken cancellationToken = default)
        {
            var exists = await _cRDBContext.LocalFileImports
                .AsNoTracking()
                .AnyAsync(x => x.BankId == bankId && x.FileName == filename, cancellationToken);
            return exists;
        }

        public Task<IEnumerable<LocalFileImportLogDto>> GetAllAsync(int Skip = 0, int Limit = 10, string? Search = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetAllCountAsync(string? Search = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<LocalFileImportLogDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<LocalFileImportLogDto> UpdateAsync(LocalFileImportLogDto entity, int Id, int UserId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
