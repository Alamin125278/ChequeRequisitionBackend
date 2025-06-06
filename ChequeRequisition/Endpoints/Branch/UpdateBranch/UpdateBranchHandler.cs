﻿using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.Branch;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;
using Mapster;
using System.Windows.Input;

namespace ChequeRequisiontService.Endpoints.Branch.UpdateBranch
{
    public record UpdateBranchCommand(int Id,int BankId, string BranchName, string BranchCode, string BranchEmail, string BranchPhone, string BranchAddress, string RoutingNo, string? IsActive = null) : ICommand<UpdateBranchResult>;
    public record UpdateBranchResult(string Message, BranchDto Branch);

    public class UpdateBranchCommandValidator : AbstractValidator<UpdateBranchCommand>
    {
        public UpdateBranchCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
            RuleFor(x => x.BankId).NotEmpty().WithMessage("Bank ID is required.");
            RuleFor(x => x.BranchName).NotEmpty().WithMessage("Branch name is required.");
            RuleFor(x => x.BranchCode).NotEmpty().WithMessage("Branch code is required.");
            RuleFor(x => x.BranchEmail).NotEmpty().EmailAddress().WithMessage("Valid email is required.");
            RuleFor(x => x.BranchPhone).NotEmpty().WithMessage("Phone number is required.");
            RuleFor(x => x.BranchAddress).NotEmpty().WithMessage("Address is required.");
            RuleFor(x => x.RoutingNo).NotEmpty().WithMessage("Routing number is required.");
        }
    }
    public class UpdateBranchHandler(IBranchRepo branchRepo, AuthenticatedUserInfo authenticatedUserInfo) : ICommandHandler<UpdateBranchCommand, UpdateBranchResult>
    {
        private readonly IBranchRepo _branchRepo = branchRepo;
        public async Task<UpdateBranchResult> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
        {
            bool activeStatus = request.IsActive == "InActive" ? false : true;
            request = request with { IsActive = null };
            var branch = request.Adapt<BranchDto>();
            branch.IsActive = activeStatus;
            var id = request.Id;
            
            var updatedBranch = await _branchRepo.UpdateAsync(branch, id, authenticatedUserInfo.Id, cancellationToken);
            
            return new UpdateBranchResult("Branch updated successfully", updatedBranch);
        }
    }
}
