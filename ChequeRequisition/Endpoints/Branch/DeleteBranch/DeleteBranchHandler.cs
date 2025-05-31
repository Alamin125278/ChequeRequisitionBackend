using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;

namespace ChequeRequisiontService.Endpoints.Branch.DeleteBranch
{
    public record DeleteBranchCommand(int Id):ICommand<DeleteBranchResult>;
    public record DeleteBranchResult(bool Success,string Message);

    public class DeleteBranchCommandValidator : AbstractValidator<DeleteBranchCommand>
    {
        public DeleteBranchCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Branch ID is required.");
        }
    }
    public class DeleteBranchHandler(IBranchRepo branchRepo, AuthenticatedUserInfo authenticatedUserInfo) :ICommandHandler<DeleteBranchCommand, DeleteBranchResult>
    {
        private readonly IBranchRepo _branchRepo = branchRepo;
        public async Task<DeleteBranchResult> Handle(DeleteBranchCommand request, CancellationToken cancellationToken)
        {
            var branch = await _branchRepo.GetByIdAsync(request.Id, cancellationToken);
            var id = request.Id;
            if (branch == null)
            {
                return new DeleteBranchResult(false, $"Branch with ID {id} not found.");
            }
            var result = await _branchRepo.DeleteAsync(id, authenticatedUserInfo.Id, cancellationToken);
            return new DeleteBranchResult(result, result ? "Branch deleted successfully" : "Failed to delete branch");
        }
    }
}
