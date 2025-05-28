using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Bank.GetAllBank
{
    public record GetAllBankQuery(int Skip, int Limit, string? Search ,string? IsActive =null) : IQuery<GetAllBankResult>;
    public record GetAllBankResult(string Meassge, IEnumerable<Core.Dto.Bank.BankDto> BankDtos,int TotalBanks);
    public class GetAllBankHandler(IBankRepo bankRepo) : IQueryHandler<GetAllBankQuery, GetAllBankResult>
    {
        private readonly IBankRepo _bankRepo = bankRepo;
        public async Task<GetAllBankResult> Handle(GetAllBankQuery request, CancellationToken cancellationToken)
        {
            bool? ActiveStatus = null;
            if (request.IsActive == "Active")
            {
                ActiveStatus = true;
            }else if (request.IsActive == "InActive")
            {
                ActiveStatus = false;
            }
            var totalCount = await _bankRepo.GetAllCountAsync(request.Search, ActiveStatus, cancellationToken);

                var banks = await _bankRepo.GetAllAsync(request.Skip, request.Limit, request.Search, ActiveStatus, cancellationToken);
            return new GetAllBankResult("Retreiving Banks Success.", banks, totalCount);
        }
    }
}
