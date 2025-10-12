using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.ManageSerial;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.ManageSerial.GetSerialByBankIdAndType;
public record GetSerialByBankIdAndTypeQuery(int BankId, string ChequeType, int Lvs):IQuery<GetSerialByBankIdAndTypeRes>;
public record GetSerialByBankIdAndTypeRes(ManageSerialDto ManageSerialDto);
public class GetSerialByBankIdAndTypeHandler(IManageSerialRepo manageSerialRepo) : IQueryHandler<GetSerialByBankIdAndTypeQuery, GetSerialByBankIdAndTypeRes>
{
    public async Task<GetSerialByBankIdAndTypeRes> Handle(GetSerialByBankIdAndTypeQuery request, CancellationToken cancellationToken)
    {
        var data = await manageSerialRepo.GetByBankIdAndChequeTypeAsync(request.BankId, request.ChequeType, request.Lvs, cancellationToken) ?? throw new Exception("No serial found for the given bank and cheque type.");
        return new GetSerialByBankIdAndTypeRes(data);
    }
}

