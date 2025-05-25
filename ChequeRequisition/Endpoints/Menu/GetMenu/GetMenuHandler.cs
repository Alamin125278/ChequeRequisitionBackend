using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Menu;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Menu.GetMenu;

public record GetMenuQuery(int Id) : IQuery<GetMenuResponse>;
public record GetMenuResponse(MenuDto? Menu);
public class GetMenuHandler(IMenuRepo menuRepo):IQueryHandler<GetMenuQuery, GetMenuResponse>
{
    public async Task<GetMenuResponse> Handle(GetMenuQuery request, CancellationToken cancellationToken)
    {
        var menu = await menuRepo.GetByIdAsync(request.Id, cancellationToken);
        if (menu == null)
        {
            throw new NotFoundException($"Menu with ID {request.Id} not found.");
        }
        return new GetMenuResponse(menu);
    }
}
