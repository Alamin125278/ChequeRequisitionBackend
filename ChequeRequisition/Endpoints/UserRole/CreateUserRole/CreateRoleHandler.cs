using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.UserRole;
using ChequeRequisiontService.Core.Interfaces.Repositories.IUserRole;
using FluentValidation;
using Mapster;
using System.Windows.Input;

namespace ChequeRequisiontService.Endpoints.UserRole.CreateUserRole;
public record CreateRoleCommand(string RoleName, bool IsActive = true):ICommand<CreateRoleResponse>;
public record CreateRoleResponse(UserRoleDto User);
public class CreateRoleValidatoe:AbstractValidator<CreateRoleCommand>
{
    public CreateRoleValidatoe()
    {
        RuleFor(x => x.RoleName).NotEmpty().WithMessage("Role name is required.");
        RuleFor(x => x.IsActive).NotNull().WithMessage("IsActive status is required.");
    }
}
public class CreateRoleHandler(IUserRoleRepo userRoleRepo):ICommandHandler<CreateRoleCommand, CreateRoleResponse>
{
    public async Task<CreateRoleResponse> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var userRole = request.Adapt<UserRoleDto>();

        var createdUserRole = await userRoleRepo.CreateAsync(userRole, 1,cancellationToken);
        
        return new CreateRoleResponse(createdUserRole);
    }
}
