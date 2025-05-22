using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Bank.GetAllBank
{
    public record GetAllBankQuery(int Skip, int Limit, string? Search) : IQuery<GetAllBankResult>;
    public record GetAllBankResult(string Meassge, IEnumerable<Core.Dto.Bank.BankDto> BankDtos);
    public class GetAllBankHandler(IBankRepo bankRepo) : IQueryHandler<GetAllBankQuery, GetAllBankResult>
    {
        private readonly IBankRepo _bankRepo = bankRepo;
        public async Task<GetAllBankResult> Handle(GetAllBankQuery request, CancellationToken cancellationToken)
        {
            var banks = await _bankRepo.GetAllAsync(request.Skip, request.Limit, request.Search, cancellationToken);
            return new GetAllBankResult("Retreiving Banks Success.", banks);
        }
    }
}
