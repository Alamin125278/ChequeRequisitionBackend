using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Menu;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;
using Mapster;
using System.Windows.Input;

namespace ChequeRequisiontService.Endpoints.Menu.UpdateMenu;

public record UpdateMenuCommand(int Id, string MenuName, string Title, string Path, string? Icon, int? ParentId, bool IsActive):ICommand<UpdateMenuResponse>;
public record UpdateMenuResponse(MenuDto Menu);

public class UpdateMenuCommandValidator : AbstractValidator<UpdateMenuCommand>
{
    public UpdateMenuCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
        RuleFor(x => x.MenuName).NotEmpty().WithMessage("Menu name is required.");
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.");
        RuleFor(x => x.Path).NotEmpty().WithMessage("Path is required.");
        RuleFor(x => x.IsActive).NotNull().WithMessage("IsActive must be specified.");
    }
}
public class UpdateMenuHandler(IMenuRepo menuRepo) : ICommandHandler<UpdateMenuCommand, UpdateMenuResponse>
{
    private readonly IMenuRepo _menuRepo = menuRepo;
    public async Task<UpdateMenuResponse> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
    {
        var menuDto = request.Adapt<MenuDto>();
        // Ensure the Id is set correctly
        var id = request.Id;
        var updatedMenu = await _menuRepo.UpdateAsync(menuDto, id, 1, cancellationToken);
        return new UpdateMenuResponse(updatedMenu);
    }
}

