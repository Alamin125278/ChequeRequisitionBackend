using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.BranchDto;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;
using Mapster;
using System.Windows.Input;

namespace ChequeRequisiontService.Endpoints.Branch.CreateBranch
{
    public record CreateBranchCommand(int BankId, string BranchName, string BranchCode, string BranchEmail, string BranchPhone, string BranchAddress, string RoutingNo):ICommand<CreateBranchResult>;
   public record CreateBranchResult(BranchDto Branch);
    public class CreateBranchCommandValidator : AbstractValidator<CreateBranchCommand>
    {
        public CreateBranchCommandValidator()
        {
            RuleFor(x => x.BankId).NotEmpty().WithMessage("Bank ID is required.");
            RuleFor(x => x.BranchName).NotEmpty().WithMessage("Branch Name is required.");
            RuleFor(x => x.BranchCode).NotEmpty().WithMessage("Branch Code is required.");
            RuleFor(x => x.BranchEmail).NotEmpty().EmailAddress().WithMessage("Valid email is required.");
            RuleFor(x => x.BranchPhone).NotEmpty().WithMessage("Phone number is required.");
            RuleFor(x => x.BranchAddress).NotEmpty().WithMessage("Address is required.");
            RuleFor(x => x.RoutingNo).NotEmpty().WithMessage("Routing Number is required.");
        }
    }
    public class CreateBranchHandler(IBranchRepo branchRepo):ICommandHandler<CreateBranchCommand, CreateBranchResult>
    {
        private readonly IBranchRepo _branchRepo = branchRepo;
        public async Task<CreateBranchResult> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
        {
            var branch = request.Adapt<BranchDto>();
            var createdBranch = await _branchRepo.CreateAsync(branch, 1, cancellationToken);
            return new CreateBranchResult(createdBranch);
        }
    }
}
