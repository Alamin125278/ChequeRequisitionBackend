using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.Challan;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Challan.GetAllChallan;
public record GetAllChallanQuery(int? BankId = null,
    int? BranchId = null, string? ChallanDate = null,
    int Skip = 0,
    int Limit = 10,
    string? Search = null) :IQuery<GetAllChallanRes>;
public record GetAllChallanRes(IEnumerable<ChallanDto> Challans,int TotalCount);

public class GetAllChallanHandler(IChallanRepo challanRepo, AuthenticatedUserInfo authenticatedUserInfo) : IQueryHandler<GetAllChallanQuery, GetAllChallanRes>
{
    public async Task<GetAllChallanRes> Handle(GetAllChallanQuery request, CancellationToken cancellationToken)
    {
        int? bankId;
        int? branchId;
        DateOnly? requestDate;
        if (!string.IsNullOrWhiteSpace(request.ChallanDate))
        {
            requestDate = DateOnly.Parse(request.ChallanDate);
        }
        else
        {
            requestDate = null;
        }
        if (authenticatedUserInfo.BankId != null)
        {
            bankId = authenticatedUserInfo.BankId;
        }
        else
        {
            bankId = request.BankId;
        }
        if (authenticatedUserInfo.BranchId != null)
        {
            branchId = authenticatedUserInfo.BranchId;
        }
        else
        {
            branchId = request.BranchId;
        }
        var challans = await challanRepo.GetAllAsync(bankId, branchId, authenticatedUserInfo.VendorId, requestDate, request.Skip, request.Limit, request.Search, cancellationToken);
        var totalCount = await challanRepo.GetAllCountAsync(bankId, branchId, authenticatedUserInfo.VendorId, requestDate, request.Search, cancellationToken);
        return new GetAllChallanRes(challans,totalCount);
    }
}
