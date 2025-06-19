using ChequeRequisiontService.Core.Dto.Ftp;
using ChequeRequisiontService.Models.CRDB;

namespace ChequeRequisiontService.Core.Interfaces.Services.FtpServices;

public interface IExcelParserService
{
    Task<IEnumerable<ChequeBookRequisition>> ParseAsync(Stream stream,FtpSetting ftp);
}
