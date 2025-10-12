using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.ManageSerial;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.ManageSerial.UpdateEndingNumber;
public record UpdateEndingNumberCommand(int BankId, string ChequeType, int Lvs, string EndingNumber) : ICommand<UpdateEndingNumberRes>;
public record UpdateEndingNumberRes(ManageSerialDto ManageSerialDto);

public class UpdateEndingNumberHandler(IManageSerialRepo manageSerialRepo, AuthenticatedUserInfo authenticatedUserInfo) : ICommandHandler<UpdateEndingNumberCommand, UpdateEndingNumberRes>
{
    public async Task<UpdateEndingNumberRes> Handle(UpdateEndingNumberCommand request, CancellationToken cancellationToken)
    {
        var data = await manageSerialRepo.UpdateAsync(request.BankId, request.ChequeType, request.EndingNumber, request.Lvs,authenticatedUserInfo.Id, cancellationToken);
        return new UpdateEndingNumberRes(data);
    }
}

