using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.UserMenuPermission;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;
using Mapster;

namespace ChequeRequisiontService.Endpoints.UserMenu.UpdateUserMenuPermission;
public record UpdateUserMenuPermissionCommand(int Id,int UserId,int MenuId, bool IsActive=true):ICommand<UpdateUserMenuPermissionResult>;
public record UpdateUserMenuPermissionResult(UserMenuPermissionDto UserMenuPermission);

public class UpdateUserMenuPermissionValidator : AbstractValidator<UpdateUserMenuPermissionCommand>
{
    public UpdateUserMenuPermissionValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("ID is required.");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");
        RuleFor(x => x.MenuId).NotEmpty().WithMessage("Menu ID is required.");
        RuleFor(x => x.IsActive).NotNull().WithMessage("IsActive is required.");
    }
}

public class UpdateUserMenuPermissionHandler(IUserMenuPermissionRepo userMenuPermissionRepo) : ICommandHandler<UpdateUserMenuPermissionCommand, UpdateUserMenuPermissionResult>
{
    public async Task<UpdateUserMenuPermissionResult> Handle(UpdateUserMenuPermissionCommand request, CancellationToken cancellationToken)
    {
        var userMenu = await userMenuPermissionRepo.GetByIdAsync(request.Id, cancellationToken);
        if (userMenu == null)
        {
            throw new KeyNotFoundException($"UserMenuPermission with Id {request.Id} not found.");
        }
        var data= request.Adapt<UserMenuPermissionDto>();
        var id= data.Id;
        var updatedUserMenu = await userMenuPermissionRepo.UpdateAsync(data, id, 1, cancellationToken);
        return new UpdateUserMenuPermissionResult(updatedUserMenu);

    }
}
