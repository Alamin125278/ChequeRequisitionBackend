using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.DefaultMenuPermission;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.DefaultMenuPermission.GetDefaultMenu;
public record getDefaultMenuQuery(int Id):IQuery<GetDefaultMenuResponse>;
public record GetDefaultMenuResponse(DefaultMenuPermisionDto? DefaultMenuPermision);
public class GetDefaultMenuHandler(IDefaultMenuPermisionRepo defaultMenuPermisionRepo) : IQueryHandler<getDefaultMenuQuery, GetDefaultMenuResponse>
{
    public async Task<GetDefaultMenuResponse> Handle(getDefaultMenuQuery request, CancellationToken cancellationToken)
    {
        var data = await defaultMenuPermisionRepo.GetByIdAsync(request.Id, cancellationToken);
        if (data == null)
        {
            throw new NotFoundException($"Default Menu Permission with ID {request.Id} not found.");
        }
        return new GetDefaultMenuResponse(data);
    }
}
