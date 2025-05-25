using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.DefaultMenuPermission;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;
using Mapster;
using System.Windows.Input;

namespace ChequeRequisiontService.Endpoints.DefaultMenuPermission.CreateDefaultMenu;
public record CreateDefaultMenuCommand(int MenuId,int RoleId,bool IsActive=true):ICommand<CreateDefaultMenuResponse>;
public record CreateDefaultMenuResponse(DefaultMenuPermisionDto DefaultMenu);

public class CreateDefaultMenuValidator : AbstractValidator<CreateDefaultMenuCommand>
{
    public CreateDefaultMenuValidator()
    {
        RuleFor(x => x.MenuId).NotEmpty().WithMessage("Menu Id is required.");
        RuleFor(x => x.RoleId).NotEmpty().WithMessage("Role Id is required.");
    }
}


public class CreateDefaultMenuHandler(IDefaultMenuPermisionRepo defaultMenuPermisionRepo) : ICommandHandler<CreateDefaultMenuCommand, CreateDefaultMenuResponse>
{
    public async Task<CreateDefaultMenuResponse> Handle(CreateDefaultMenuCommand request, CancellationToken cancellationToken)
    {
       var defaultMenu = request.Adapt<DefaultMenuPermisionDto>();
        var createdDefaultMenu = await defaultMenuPermisionRepo.CreateAsync(defaultMenu, 1, cancellationToken);
        return new CreateDefaultMenuResponse(createdDefaultMenu);
    }
}
