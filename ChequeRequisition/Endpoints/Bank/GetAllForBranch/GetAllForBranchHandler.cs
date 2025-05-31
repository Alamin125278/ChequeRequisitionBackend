using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.Bank;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Bank.GetAllForBranch;
public record GetAllForBranchQuery(): IQuery<GetAllForBranchResult>;
public record GetAllForBranchResult(IEnumerable<BankDto> BankDtos);
public class GetAllForBranchHandler(IBankRepo bankRepo, AuthenticatedUserInfo authenticatedUserInfo):IQueryHandler<GetAllForBranchQuery, GetAllForBranchResult>
{
    private readonly IBankRepo _bankRepo = bankRepo;
    public async Task<GetAllForBranchResult> Handle(GetAllForBranchQuery request, CancellationToken cancellationToken)
    {
        var bankId = authenticatedUserInfo.BankId;
        var banks = await _bankRepo.GetAllAsync(bankId, cancellationToken);
        return new GetAllForBranchResult(banks);
    }
}
