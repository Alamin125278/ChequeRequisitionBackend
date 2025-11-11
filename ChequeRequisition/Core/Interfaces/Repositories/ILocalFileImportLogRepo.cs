using ChequeRequisiontService.Core.Dto.LocalFileImportLog;

namespace ChequeRequisiontService.Core.Interfaces.Repositories
{
    public interface ILocalFileImportLogRepo:IGenericRepository<LocalFileImportLogDto>
    {
        Task<bool> ExistsAsync(int bankId, string filename, CancellationToken cancellationToken = default);
    }
}
