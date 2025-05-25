using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;
using System.Windows.Input;

namespace ChequeRequisiontService.Endpoints.DefaultMenuPermission.DeleteDefaultMenu;
public record DeleteDefaultMenuCommand(int Id):ICommand<DeleteDefaultMenuResponse>;
public record DeleteDefaultMenuResponse(bool IsSuccess, string Message);

public class  DeleteDefaultMenuValidator:AbstractValidator<DeleteDefaultMenuCommand>
{
    public DeleteDefaultMenuValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id must be greater than 0");
    }
}
public class DeleteDefaultMenuHandler(IDefaultMenuPermisionRepo defaultMenuPermisionRepo) : ICommandHandler<DeleteDefaultMenuCommand, DeleteDefaultMenuResponse>
{
    public async Task<DeleteDefaultMenuResponse> Handle(DeleteDefaultMenuCommand request, CancellationToken cancellationToken)
    {
        var DefaultMenuPermission = await defaultMenuPermisionRepo.GetByIdAsync(request.Id, cancellationToken);
        if (DefaultMenuPermission == null)
        {
            return new DeleteDefaultMenuResponse(false, $"Default menu permission with ID {request.Id} not found.");
        }
        var result = await defaultMenuPermisionRepo.DeleteAsync(request.Id, 1, cancellationToken);
        if (result)
        {
            return new DeleteDefaultMenuResponse(true, "Default menu permission deleted successfully.");
        }
        return new DeleteDefaultMenuResponse(false, "Failed to delete default menu permission.");
    }
}
