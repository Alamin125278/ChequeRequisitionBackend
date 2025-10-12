using ChequeRequisiontService.Core.Dto.ManageSerial;

namespace ChequeRequisiontService.Core.Interfaces.Repositories;

public interface IManageSerialRepo
{
    Task<ManageSerialDto> GetByBankIdAndChequeTypeAsync(int BankId,string ChequeType,int Lvs,CancellationToken cancellationToken=default );
    Task<ManageSerialDto> UpdateAsync(int BankId,string ChequeType,string EndingNumber,int Lvs,int UserId,CancellationToken cancellationToken=default);
}
