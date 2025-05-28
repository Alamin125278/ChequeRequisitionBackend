using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;
using System.Windows.Input;

namespace ChequeRequisiontService.Endpoints.UserMenu.DeleteUserMenuPermission;
public record DeleteUserMenuPermissionCommand(int Id) : ICommand<DeleteUserMenuPermissionResponse>;
public record DeleteUserMenuPermissionResponse(bool Success,string Message);
public class DeleteUserMenuPermissionValidator : AbstractValidator<DeleteUserMenuPermissionCommand>
{
    public DeleteUserMenuPermissionValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("User Menu Permission ID must be greater than 0.");
    }
}
public class DeleteUserMenuPermissionHandler(IUserMenuPermissionRepo userMenuPermissionRepo) : ICommandHandler<DeleteUserMenuPermissionCommand, DeleteUserMenuPermissionResponse>
{
    public async Task<DeleteUserMenuPermissionResponse> Handle(DeleteUserMenuPermissionCommand request, CancellationToken cancellationToken)
    {
       var data = await userMenuPermissionRepo.GetByIdAsync(request.Id);
        if (data == null)
        {
            return new DeleteUserMenuPermissionResponse(false, $"User Menu Permission with ID {request.Id} not found.");
        }
        
        var id = request.Id;
        await userMenuPermissionRepo.DeleteAsync(id, 1, cancellationToken);
        
        return new DeleteUserMenuPermissionResponse(true, $"User Menu Permission with ID {request.Id} deleted successfully.");
    }
}
