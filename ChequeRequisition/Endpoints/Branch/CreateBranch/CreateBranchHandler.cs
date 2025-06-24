using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.Branch;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;
using Mapster;

namespace ChequeRequisiontService.Endpoints.Branch.CreateBranch
{
    public record CreateBranchCommand(int BankId, string BranchName, string BranchCode, string BranchEmail, string BranchPhone, string BranchAddress, string RoutingNo, string? IsActive = null) :ICommand<CreateBranchResult>;
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
    public class CreateBranchHandler(IBranchRepo branchRepo, AuthenticatedUserInfo authenticatedUserInfo) :ICommandHandler<CreateBranchCommand, CreateBranchResult>
    {
        private readonly IBranchRepo _branchRepo = branchRepo;
        public async Task<CreateBranchResult> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
        {
            bool activeStatus = request.IsActive == "InActive" ? false : true;
            request = request with { IsActive = null };
            var branch = request.Adapt<BranchDto>();
            branch.IsActive = activeStatus;
            var createdBranch = await _branchRepo.CreateAsync(branch, authenticatedUserInfo.Id, cancellationToken);
            return new CreateBranchResult(createdBranch);
        }
    }
}
