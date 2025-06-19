using ChequeRequisiontService.Core.Dto.Ftp;
using ChequeRequisiontService.Core.Dto.Requisition;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.DbContexts;
using ChequeRequisiontService.Models.CRDB;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Mapster;
using Microsoft.EntityFrameworkCore;
using static Dapper.SqlMapper;

namespace ChequeRequisiontService.Infrastructure.Repositories.FtpImportExcel;

public class FtpImportRepo(CRDBContext cRDBContext) : IFtpImportRepo
{
    private readonly CRDBContext _cRDBContext = cRDBContext;

    public async Task<List<int>> BulkInsertRequisitionsAsync(IEnumerable<ChequeBookRequisition> list, CancellationToken cancellationToken = default)
    {
        await _cRDBContext.ChequeBookRequisitions.AddRangeAsync(list, cancellationToken);
        var result= await _cRDBContext.SaveChangesAsync(cancellationToken);
        if(result > 0)
        {
            return list.Select(x => x.Id).ToList();
        }
        throw new Exception("Failed to bulk insert cheque book requisitions");
    }


    public async Task<int> InsertFtpLogAsync(FtpImportLogDto log, CancellationToken cancellationToken = default)
    {
        var ftpLog = log.Adapt<FtpImport>();
        ftpLog.CreatedAt = DateTime.UtcNow;
        await _cRDBContext.FtpImports.AddAsync(ftpLog, cancellationToken);
        var result = await _cRDBContext.SaveChangesAsync(cancellationToken);
        if (result > 0)
        {
            return ftpLog.Id;
        }
        throw new Exception("Failed to insert FTP log");
    }


}
