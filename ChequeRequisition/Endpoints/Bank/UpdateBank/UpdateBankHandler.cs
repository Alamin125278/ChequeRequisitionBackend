using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.Bank;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;
using Mapster;

namespace ChequeRequisiontService.Endpoints.Bank.UpdateBank
{
    public record UpdateBankCommand(int Id, int VendorId, string BankName, string BankCode, string RoutingNumber, string BankEmail, string BankPhone, string BankAddress,string? IsActive=null) : ICommand<UpdateBankResult>;
    public record UpdateBankResult(BankDto Bank);
    public class UpdateBankCommandValidator : AbstractValidator<UpdateBankCommand>
    {
        public UpdateBankCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
            RuleFor(x => x.VendorId).NotEmpty().WithMessage("Vendor ID is required.");
            RuleFor(x => x.BankName).NotEmpty().WithMessage("Bank name is required.");
            RuleFor(x => x.BankCode).NotEmpty().WithMessage("Bank code is required.");
            RuleFor(x => x.RoutingNumber).NotEmpty().WithMessage("Routing number is required.");
            RuleFor(x => x.BankEmail).NotEmpty().EmailAddress().WithMessage("Valid email is required.");
            RuleFor(x => x.BankPhone).NotEmpty().WithMessage("Phone number is required.");
            RuleFor(x => x.BankAddress).NotEmpty().WithMessage("Address is required.");
        }
    }
    public class UpdateBankHandler(IBankRepo bankRepo, AuthenticatedUserInfo authenticatedUserInfo) : ICommandHandler<UpdateBankCommand, UpdateBankResult>
    {
        private readonly IBankRepo _bankRepo = bankRepo;
        public async Task<UpdateBankResult> Handle(UpdateBankCommand request, CancellationToken cancellationToken)
        {
            bool activeStatus = request.IsActive == "InActive" ? false : true;
            request = request with { IsActive = null };

            var bank = request.Adapt<BankDto>();
            bank.IsActive = activeStatus;
            bank.VendorId = request.VendorId;
            var id = request.Id;
            
            var updatedBank = await _bankRepo.UpdateAsync(bank, id, authenticatedUserInfo.Id, cancellationToken);
            
            return new UpdateBankResult(updatedBank);
        }
    }
}
