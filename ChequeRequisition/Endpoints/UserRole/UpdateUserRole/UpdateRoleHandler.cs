using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.UserRole;
using ChequeRequisiontService.Core.Interfaces.Repositories.IUserRole;
using FluentValidation;
using Mapster;

namespace ChequeRequisiontService.Endpoints.UserRole.UpdateUserRole;
public record UpdateRoleCommand(int Id, string RoleName, bool IsActive=true) : ICommand<UpdateRoleResponse>;
public record UpdateRoleResponse(UserRoleDto UserRole);
public class UpdateRoleValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
        RuleFor(x => x.RoleName).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.IsActive).NotNull().WithMessage("IsActive is required.");
    }
}
public class UpdateRoleHandler(IUserRoleRepo userRoleRepo) : ICommandHandler<UpdateRoleCommand, UpdateRoleResponse>
{
    private readonly IUserRoleRepo _userRoleRepo = userRoleRepo;
    
    public async Task<UpdateRoleResponse> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var userRole = request.Adapt<UserRoleDto>();
        var id = request.Id;
        
        var updatedUserRole = await _userRoleRepo.UpdateAsync(userRole, id,1, cancellationToken);
        
        return new UpdateRoleResponse(updatedUserRole);
    }
}
