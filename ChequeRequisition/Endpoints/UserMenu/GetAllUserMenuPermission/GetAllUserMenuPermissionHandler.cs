using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.UserMenuPermission;
using ChequeRequisiontService.Core.Interfaces.Repositories;

namespace ChequeRequisiontService.Endpoints.UserMenu.GetAllUserMenuPermission;
public record GetAllUserMenuPermissionQuery(int Skip=0,int Limit=10,string? Search=null):IQuery<GetAllUserMenuPermissionResult>;
public record GetAllUserMenuPermissionResult(string Message, IEnumerable<UserMenuPermissionDto> UserMenuPermissions);
public class GetAllUserMenuPermissionHandler(IUserMenuPermissionRepo userMenuPermissionRepo) : IQueryHandler<GetAllUserMenuPermissionQuery, GetAllUserMenuPermissionResult>
{
    private readonly IUserMenuPermissionRepo _userMenuPermissionRepo= userMenuPermissionRepo;
    public async Task<GetAllUserMenuPermissionResult> Handle(GetAllUserMenuPermissionQuery request, CancellationToken cancellationToken)
    {
        var userMenuPermissions = await _userMenuPermissionRepo.GetAllAsync(request.Skip, request.Limit, request.Search);
        return new GetAllUserMenuPermissionResult("Retrieving User Menu Permission Success.", userMenuPermissions);
    }
}
