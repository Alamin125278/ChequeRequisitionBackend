using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.Menu;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;
using Mapster;
using System.Windows.Input;

namespace ChequeRequisiontService.Endpoints.Menu.CreateMenu
{
    public record CreateMenuCommand(string MenuName,string Title, string Path, string? Icon, int? ParentId):ICommand<CreateMenuResponse>;
    public record CreateMenuResponse(MenuDto Menu);
    public class CreateMenuValidator : AbstractValidator<CreateMenuCommand>
    {
        public CreateMenuValidator()
        {
            RuleFor(x => x.MenuName).NotEmpty().WithMessage("Menu name is required.");
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.");
            RuleFor(x => x.Path).NotEmpty().WithMessage("Path is required.");
        }
    }
    public class CreateMenuHandler(IMenuRepo menuRepo,AuthenticatedUserInfo authenticatedUserInfo) : ICommandHandler<CreateMenuCommand, CreateMenuResponse>
    {
        private readonly IMenuRepo _menuRepo = menuRepo;
        public async Task<CreateMenuResponse> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
        {
            var menu = request.Adapt<MenuDto>();

            var createdMenu = await _menuRepo.CreateAsync(menu, authenticatedUserInfo.Id, cancellationToken);
            
            return new CreateMenuResponse(createdMenu);
        }
    }
}
