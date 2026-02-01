using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.UserMenuPermission;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.DbContexts;
using ChequeRequisiontService.Models.CRDB;
using DocumentFormat.OpenXml.Spreadsheet;

namespace ChequeRequisiontService.Endpoints.UserMenusPermission.CreateUserMenuPermissions;
public record CreateUserMenuPermissionsCommand(int UserId,
    IReadOnlyList<UserMenuAccessDto> Payload) : ICommand<CreateUserMenuPermissionRes>;

public record CreateUserMenuPermissionRes(string Message);

public class CreateUserMenuPermissionsHandler(IUserMenuPermissionRepo userMenuPermissionRepo, CRDBContext cRDBContext,AuthenticatedUserInfo authenticatedUserInfo) : ICommandHandler<CreateUserMenuPermissionsCommand, CreateUserMenuPermissionRes>
{
    public async Task<CreateUserMenuPermissionRes> Handle(CreateUserMenuPermissionsCommand request, CancellationToken cancellationToken)
    {
        var createList = request.Payload
           .Where(x => x.CanAccess)
           .Select(x => new UserMenuPermission
           {
               UserId = request.UserId,
               MenuId = x.MenuId,
               IsActive = true,
               IsDeleted = false,
               CreatedBy = authenticatedUserInfo.Id,
               CreatedAt = DateTime.UtcNow
           })
           .ToList();

        var deleteMenuIds = request.Payload
            .Where(x => !x.CanAccess)
            .Select(x => x.MenuId)
            .ToList();
        using var transaction = await cRDBContext.Database
           .BeginTransactionAsync(cancellationToken);

        try
        {
            if (createList.Count != 0)
                await userMenuPermissionRepo.BulkCreateAsync(createList, cancellationToken);

            if (deleteMenuIds.Count != 0)
                await userMenuPermissionRepo.BulkSoftDeleteAsync(
                    request.UserId,
                    deleteMenuIds,
                    authenticatedUserInfo.Id,
                    cancellationToken);

            await cRDBContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new CreateUserMenuPermissionRes(
                "User menu permissions override processed successfully.");
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
