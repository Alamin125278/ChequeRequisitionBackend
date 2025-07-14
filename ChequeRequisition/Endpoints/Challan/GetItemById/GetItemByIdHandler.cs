using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Challan;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Challan.GetItemById;

public record GetItemByIdQuery(int Id) : IQuery<GetItemByIdResult>;
public record GetItemByIdResult(IEnumerable<ChallanItemDto> ChallanItems,int TotalCount);
public class GetItemByIdHandler(IChallanRepo challanRepo) : IQueryHandler<GetItemByIdQuery, GetItemByIdResult>
{
    public async Task<GetItemByIdResult> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        var items = await challanRepo.GetAllItemAsync(request.Id, cancellationToken);
        var totalCount = await challanRepo.GetAllItemCountAsync(request.Id, cancellationToken);
        return new GetItemByIdResult(items,totalCount);
    }
}
