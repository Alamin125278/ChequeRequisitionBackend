using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;

namespace ChequeRequisiontService.Endpoints.User.DeleteUser
{
    public record DeleteUserCommand(int Id) : ICommand<DeleteUserResult>;   
    public record DeleteUserResult(bool Success, string Message);
    public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("User ID must be greater than 0.");
        }
    }
    public class DeleteHandler(IUserRepo userRepo,AuthenticatedUserInfo authenticatedUserInfo) : ICommandHandler<DeleteUserCommand, DeleteUserResult>
    {
        public async Task<DeleteUserResult> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepo.GetByIdAsync(request.Id, cancellationToken);
            var id = request.Id;
            if (user == null)
            {
                return new DeleteUserResult(false, $"User with ID {request.Id} not found.");
            }
            var result = await userRepo.DeleteAsync(id, authenticatedUserInfo.Id, cancellationToken);
            return new DeleteUserResult(result, result? $"User with ID {request.Id} deleted successfully.":"Failed Delete User");
        }
    }
}
