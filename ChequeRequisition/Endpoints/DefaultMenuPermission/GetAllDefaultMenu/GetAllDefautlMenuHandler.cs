using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.DefaultMenuPermission;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.DefaultMenuPermission.GetAllDefaultMenu;
public record GetAllDefautlMenuQuery(int Skip = 0, int Limit = 10, string? Search = null):IQuery<GetAllDefautlMenuQueryResponse>;
public record GetAllDefautlMenuQueryResponse(string Message,IEnumerable<DefaultMenuPermisionDto> DefaultMenuPermissions);
public class GetAllDefautlMenuHandler(IDefaultMenuPermisionRepo defaultMenuPermisionRepo) : IQueryHandler<GetAllDefautlMenuQuery, GetAllDefautlMenuQueryResponse>
{
    public async Task<GetAllDefautlMenuQueryResponse> Handle(GetAllDefautlMenuQuery request, CancellationToken cancellationToken)
    {
        var data = await defaultMenuPermisionRepo.GetAllAsync(request.Skip, request.Limit, request.Search, cancellationToken);
        return new GetAllDefautlMenuQueryResponse("Default Menu Permissions retrieved successfully", data);
    }
}
