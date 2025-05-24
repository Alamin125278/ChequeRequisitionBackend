using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.Bank;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Bank.GetBank
{
    public record GetBankQuery(int Id) : IQuery<GetBankResult>;
    public record GetBankResult(BankDto? Bank);

    public class GetBankHandler(IBankRepo bankRepo, AuthenticatedUserInfo authenticatedUserInfo) : IQueryHandler<GetBankQuery, GetBankResult>
    {
        public async Task<GetBankResult> Handle(GetBankQuery request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"GetBankHandler: Handling request for bank with ID {request.Id} by user {authenticatedUserInfo.UserName}.");
            var bank = await bankRepo.GetByIdAsync(request.Id, cancellationToken);

            if (bank == null)
            {
                throw new NotFoundException($"Bank with ID {request.Id} not found.");
            }

            return new GetBankResult(bank);
        }
    }
}
