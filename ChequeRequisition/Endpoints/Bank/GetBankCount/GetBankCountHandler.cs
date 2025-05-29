using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Bank.GetBankCount;
public record GetBankCountQuery() : IQuery<GetBankCountResult>;
public record GetBankCountResult(int TotalBank,int ActiveBank);

public class GetBankCountHandler(IBankRepo bankRepo) : IQueryHandler<GetBankCountQuery, GetBankCountResult>
{
    private readonly IBankRepo _bankRepo = bankRepo;
    public async Task<GetBankCountResult> Handle(GetBankCountQuery request, CancellationToken cancellationToken)
    {
        var totalCount = await _bankRepo.GetAllCountAsync(cancellationToken: cancellationToken);
        var activeCount = await _bankRepo.GetAllCountAsync(IsActive: true, cancellationToken: cancellationToken);
        
        return new GetBankCountResult(totalCount, activeCount);
    }
}
