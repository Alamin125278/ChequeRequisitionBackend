using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Menu;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.Menu.GetAllMenu;

public record GetAllMenuQuery(int Skip = 0, int Limit = 10, string? Search = null):IQuery<GetAllMenuResponse>;
public record GetAllMenuResponse(string Message, IEnumerable<MenuDto> Menus);
public class GetAllMenuHandler(IMenuRepo menuRepo) : IQueryHandler<GetAllMenuQuery, GetAllMenuResponse>
{
    private readonly IMenuRepo _menuRepo = menuRepo;
    public async Task<GetAllMenuResponse> Handle(GetAllMenuQuery request, CancellationToken cancellationToken)
    {
        var menus = await _menuRepo.GetAllAsync(request.Skip, request.Limit, request.Search, cancellationToken);
        return new GetAllMenuResponse("Retrieving Menus Success.", menus);
    }
}
