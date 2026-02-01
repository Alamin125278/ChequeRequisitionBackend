using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Menu;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.Infrastructure.Repositories.BranchRepo;
using ChequeRequisiontService.Models.CRDB;

namespace ChequeRequisiontService.Endpoints.Menu.GetAllMenu;

public record GetAllMenuQuery(int Skip = 0, int Limit = 10, string? Search = null):IQuery<GetAllMenuResponse>;
public record GetAllMenuResponse(string Message, IEnumerable<MenuDto> Menus, int TotalMenus);
public class GetAllMenuHandler(IMenuRepo menuRepo) : IQueryHandler<GetAllMenuQuery, GetAllMenuResponse>
{
    private readonly IMenuRepo _menuRepo = menuRepo;
    public async Task<GetAllMenuResponse> Handle(GetAllMenuQuery request, CancellationToken cancellationToken)
    {
        var totalCount = await _menuRepo.GetAllCountAsync(request.Search,null, cancellationToken);
        var menus = await _menuRepo.GetAllAsync(request.Skip, request.Limit, request.Search, cancellationToken);
        return new GetAllMenuResponse("Retrieving Menus Success.", menus, totalCount);
    }
}
