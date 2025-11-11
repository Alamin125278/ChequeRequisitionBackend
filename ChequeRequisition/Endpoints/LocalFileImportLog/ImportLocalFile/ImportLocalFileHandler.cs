using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.LocalFileImportLog;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.Endpoints.Branch.CreateBranch;
using Mapster;
using MediatR;
using System.Windows.Input;

namespace ChequeRequisiontService.Endpoints.LocalFileImportLog.ImportLocalFile;
public record  ImportLocalFileCommand(int BankId,string FileName):ICommand<ImportLocalFileRes>;
 public record ImportLocalFileRes(LocalFileImportLogDto LocalFileImportLog);

public class ImportLocalFileHandler(ILocalFileImportLogRepo localFileImportLogRepo, AuthenticatedUserInfo authenticatedUserInfo) : ICommandHandler<ImportLocalFileCommand,ImportLocalFileRes>
{
    public async Task<ImportLocalFileRes> Handle(ImportLocalFileCommand request, CancellationToken cancellationToken)
    {
        var localFileImportLogDto = request.Adapt<LocalFileImportLogDto>();
        var createdLog = await  localFileImportLogRepo.CreateAsync(localFileImportLogDto, authenticatedUserInfo.Id, cancellationToken);
        return new ImportLocalFileRes(createdLog);
    }
}
