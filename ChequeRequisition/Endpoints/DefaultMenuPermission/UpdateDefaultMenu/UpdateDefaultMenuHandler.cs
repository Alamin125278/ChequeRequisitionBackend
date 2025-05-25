using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.DefaultMenuPermission;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;
using Mapster;

namespace ChequeRequisiontService.Endpoints.DefaultMenuPermission.UpdateDefaultMenu;
public record UpdateDefaultMenuCommand(int Id, int MenuId, int RoleId,bool IsActive=true) : ICommand<UpdateDefaultMenuResponse>;
public record UpdateDefaultMenuResponse(DefaultMenuPermisionDto DefaultMenuPermisionDto);

public class UpdateDefaultMenuCommandValidator : AbstractValidator<UpdateDefaultMenuCommand>
{
    public UpdateDefaultMenuCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
        RuleFor(x => x.MenuId).NotEmpty().WithMessage("MenuId is required.");
        RuleFor(x => x.RoleId).NotEmpty().WithMessage("RoleId is required.");
        RuleFor(x => x.IsActive).NotNull().WithMessage("IsActive must be specified.");
    }
}
public class UpdateDefaultMenuHandler(IDefaultMenuPermisionRepo defaultMenuPermisionRepo):ICommandHandler<UpdateDefaultMenuCommand, UpdateDefaultMenuResponse>
{
    private readonly IDefaultMenuPermisionRepo _defaultMenuPermisionRepo = defaultMenuPermisionRepo;
    
    public async Task<UpdateDefaultMenuResponse> Handle(UpdateDefaultMenuCommand request, CancellationToken cancellationToken)
    {
        var defaultMenuPermisionDto = request.Adapt<DefaultMenuPermisionDto>();
        // Ensure the Id is set correctly
        var id = request.Id;
        var updatedDefaultMenu = await _defaultMenuPermisionRepo.UpdateAsync(defaultMenuPermisionDto, id, 1, cancellationToken);
        return new UpdateDefaultMenuResponse(updatedDefaultMenu);
    }
}
