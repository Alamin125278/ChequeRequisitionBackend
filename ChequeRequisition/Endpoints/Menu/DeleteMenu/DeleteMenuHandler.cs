using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;
using System.Windows.Input;

namespace ChequeRequisiontService.Endpoints.Menu.DeleteMenu
{
    public record DeleteMenuCommand(int Id):ICommand<DeleteMenuResponse>;
    public record DeleteMenuResponse(bool Success,string Message);
    public class DeleteMenuCommandValidator : AbstractValidator<DeleteMenuCommand>
    {
        public DeleteMenuCommandValidator() 
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Menu ID must be greater than 0.");
        }
    }
    public class DeleteMenuHandler(IMenuRepo menuRepo) : ICommandHandler<DeleteMenuCommand, DeleteMenuResponse>
    {
        public async Task<DeleteMenuResponse> Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
        {
            var menu = await menuRepo.GetByIdAsync(request.Id, cancellationToken);
            if (menu == null)
            {
                return new DeleteMenuResponse(false, $"Menu with ID {request.Id} not found.");
            }
            await menuRepo.DeleteAsync(request.Id, 1, cancellationToken);
            return new DeleteMenuResponse(true, $"Menu with ID {request.Id} deleted successfully.");
        }
    }
}
