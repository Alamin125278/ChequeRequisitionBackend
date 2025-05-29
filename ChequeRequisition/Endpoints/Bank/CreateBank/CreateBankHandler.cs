using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.Bank;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;
using Mapster;
using System.Windows.Input;

namespace ChequeRequisiontService.Endpoints.Bank.CreateBank
{
    public record CreateBankCommand(int VendorId, string BankName, string BankCode, string RoutingNumber, string BankEmail, string BankPhone, string BankAddress,string ? IsActive=null):ICommand<CreateBankResult>;
   public record CreateBankResult(BankDto Bank);

    public class CreateBankValidator : AbstractValidator<CreateBankCommand>
    {
        public CreateBankValidator()
        {
            RuleFor(x => x.VendorId).NotEmpty().WithMessage("Vendor ID is required.");
            RuleFor(x => x.BankName).NotEmpty().WithMessage("Bank Name is required.");
            RuleFor(x => x.BankCode).NotEmpty().WithMessage("Bank Code is required.");
            RuleFor(x => x.RoutingNumber).NotEmpty().WithMessage("Routing Number is required.");
            RuleFor(x => x.BankEmail).NotEmpty().EmailAddress().WithMessage("Valid email is required.");
            RuleFor(x => x.BankPhone).NotEmpty().WithMessage("Phone number is required.");
            RuleFor(x => x.BankAddress).NotEmpty().WithMessage("Address is required.");
        }
    }
    public class CreateBankHandler(IBankRepo bankRepo, AuthenticatedUserInfo authenticatedUserInfo):ICommandHandler<CreateBankCommand, CreateBankResult>
    {
        private readonly IBankRepo _bankRepo = bankRepo;
        public async Task<CreateBankResult> Handle(CreateBankCommand request, CancellationToken cancellationToken)
        {
            bool activeStatus = request.IsActive == "InActive" ? false : true;
            request = request with { IsActive = null };
            var bank = request.Adapt<BankDto>();
            bank.IsActive = activeStatus;
            var createdBank = await _bankRepo.CreateAsync(bank, authenticatedUserInfo.Id, cancellationToken);
            return new CreateBankResult(createdBank);
        }
    }
}
