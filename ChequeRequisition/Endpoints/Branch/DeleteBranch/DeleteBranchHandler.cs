using BuildingBlocks.CQRS;
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
    public class DeleteBranchHandler(IBranchRepo branchRepo):ICommandHandler<DeleteBranchCommand, DeleteBranchResult>
    {
        private readonly IBranchRepo _branchRepo = branchRepo;
        public async Task<DeleteBranchResult> Handle(DeleteBranchCommand request, CancellationToken cancellationToken)
        {
            var result = await _branchRepo.DeleteAsync(request.Id, 1, cancellationToken);
            return new DeleteBranchResult(result, result ? "Branch deleted successfully" : "Failed to delete branch");
        }
    }
}
