using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Interfaces.Repositories.IUserRole;
using FluentValidation;
using System.Windows.Input;

namespace ChequeRequisiontService.Endpoints.UserRole.DeleteUserRole;
public record DeleteRoleCommand(int Id):ICommand<DeleteRoleResponse>;
public record DeleteRoleResponse(bool Success, string Message);

public class DeleteRoleValidator : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Role ID must be greater than 0.");
    }
}
public class DeleteRoleHandler(IUserRoleRepo userRoleRepo) : ICommandHandler<DeleteRoleCommand, DeleteRoleResponse>
{
    public async Task<DeleteRoleResponse> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await userRoleRepo.GetByIdAsync(request.Id);
        var id = request.Id;
        if (role == null)
        {
            return new DeleteRoleResponse(false, $"Role with ID {request.Id} not found.");
        }
        await userRoleRepo.DeleteAsync(id,1,cancellationToken);
        return new DeleteRoleResponse(true, $"Role with ID {request.Id} deleted successfully.");
    }
}
