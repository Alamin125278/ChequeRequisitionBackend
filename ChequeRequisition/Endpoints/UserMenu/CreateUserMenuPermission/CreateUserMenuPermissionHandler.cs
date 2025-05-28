using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.UserMenuPermission;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;
using Mapster;
using System.Windows.Input;

namespace ChequeRequisiontService.Endpoints.UserMenu.CreateUserMenuPermission;
public record CreateUserMenuPermissionCommand(int UserId,int MenuId,bool IsActive=true):ICommand<CreateUserMenuPermissionResult>;
public record CreateUserMenuPermissionResult(UserMenuPermissionDto UserMenuPermission);
public class CreateUserMenuPermissionValidator : AbstractValidator<CreateUserMenuPermissionCommand>
{
    public CreateUserMenuPermissionValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
        RuleFor(x => x.MenuId).NotEmpty().WithMessage("MenuId is required.");
        RuleFor(x => x.IsActive).NotNull().WithMessage("IsActive must be specified.");
    }
}


public class CreateUserMenuPermissionHandler(IUserMenuPermissionRepo userMenuPermissionRepo) : ICommandHandler<CreateUserMenuPermissionCommand, CreateUserMenuPermissionResult>
{
    public async Task<CreateUserMenuPermissionResult> Handle(CreateUserMenuPermissionCommand request, CancellationToken cancellationToken)
    {
        var data = request.Adapt<UserMenuPermissionDto>();
        var createdUserMenuPermission = await userMenuPermissionRepo.CreateAsync(data, 1, cancellationToken);
        return new CreateUserMenuPermissionResult(createdUserMenuPermission);
    }
}
