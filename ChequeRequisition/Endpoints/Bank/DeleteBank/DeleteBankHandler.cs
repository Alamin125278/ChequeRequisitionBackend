using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;

namespace ChequeRequisiontService.Endpoints.Bank.DeleteBank
{
    public record DeleteBankCommand(int Id):ICommand<DeleteBankResult>;
    public record DeleteBankResult(bool Success,string Message);

    public class DeleteBankCommandValidator : AbstractValidator<DeleteBankCommand>
    {
        public DeleteBankCommandValidator() 
        {
            RuleFor(x=>x.Id).GreaterThan(0).WithMessage("Bank ID must be greater than 0.");
        }
    }
    public class DeleteBankHandler(IBankRepo bankRepo, AuthenticatedUserInfo authenticatedUserInfo) : ICommandHandler<DeleteBankCommand, DeleteBankResult>
    {
        public async Task<DeleteBankResult> Handle(DeleteBankCommand request, CancellationToken cancellationToken)
        {
            var bank = await bankRepo.GetByIdAsync(request.Id, cancellationToken);
            var id = request.Id;
            if (bank == null)
            {
                return new DeleteBankResult(false, $"Bank with ID {request.Id} not found.");
            }
            await bankRepo.DeleteAsync(id, authenticatedUserInfo.Id, cancellationToken);
            return new DeleteBankResult(Success:true, Message:$"Bank with ID {request.Id} deleted successfully.");

        }
    }
}
