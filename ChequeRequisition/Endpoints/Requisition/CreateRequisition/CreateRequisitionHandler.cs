using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.Requisition;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;
using Mapster;

namespace ChequeRequisiontService.Endpoints.Requisition.CreateRequisition
{
    public record CreateRequisitionCommand(int BankId, int BranchId, string AccountNo, string RoutingNo, string StartNo, string EndNo, string ChequeType, string ChequePrefix, string MicrNo, string Series, string AccountName, string CusAddress, int BookQty, int TransactionCode, int Leaves, int CourierCode, int ReceivingBranchId, string RequestDate, int Serverity):ICommand<CreateRequisitionResult>;
   public record CreateRequisitionResult(RequisitionDto RequisitionDto);
    public class CreateRequisitionCommandValidator : AbstractValidator<CreateRequisitionCommand>
    {
        public CreateRequisitionCommandValidator()
        {
            RuleFor(x => x.BankId).NotEmpty().WithMessage("Bank ID is required.");
            RuleFor(x => x.BranchId).NotEmpty().WithMessage("Branch ID is required.");
            RuleFor(x => x.AccountNo).NotEmpty().WithMessage("Account Number is required.");
            RuleFor(x => x.RoutingNo).NotEmpty().WithMessage("Routing Number is required.");
            RuleFor(x => x.StartNo).NotEmpty().WithMessage("Start Number is required.");
            RuleFor(x => x.EndNo).NotEmpty().WithMessage("End Number is required.");
            RuleFor(x => x.ChequeType).NotEmpty().WithMessage("Cheque Type is required.");
            RuleFor(x => x.ChequePrefix).NotEmpty().WithMessage("Cheque Prefix is required.");
            RuleFor(x => x.MicrNo).NotEmpty().WithMessage("MICR Number is required.");
            RuleFor(x => x.Series).NotEmpty().WithMessage("Series is required.");
            RuleFor(x => x.AccountName).NotEmpty().WithMessage("Account Name is required.");
            RuleFor(x => x.CusAddress).NotEmpty().WithMessage("Customer Address is required.");
            RuleFor(x => x.BookQty).NotEmpty().WithMessage("Book Quantity is required.");
            RuleFor(x => x.TransactionCode).NotEmpty().WithMessage("Transaction Code is required.");
            RuleFor(x => x.Leaves).NotEmpty().WithMessage("Leaves are required.");
            RuleFor(x => x.CourierCode).NotEmpty().WithMessage("Courier Code is required.");
            RuleFor(x => x.ReceivingBranchId).NotEmpty().WithMessage("Receiving Branch ID is required.");
            RuleFor(x => x.RequestDate).NotEmpty().WithMessage("Requisition Date is required.");
            RuleFor(x => x.Serverity).NotEmpty().WithMessage("Serverity is required.");
        }
    }
    public class CreateRequisitionHandler(IRequisitonRepo requisitonRepo,AuthenticatedUserInfo authenticatedUserInfo):ICommandHandler<CreateRequisitionCommand, CreateRequisitionResult>
    {
        private readonly IRequisitonRepo _requisitonRepo = requisitonRepo;
        public async Task<CreateRequisitionResult> Handle(CreateRequisitionCommand request, CancellationToken cancellationToken)
        {
            
            var requisition = request.Adapt<RequisitionDto>();
            //requisition.RequestDate = DateOnly.Parse(request.RequestDate);


            var createdRequisition = await _requisitonRepo.CreateAsync(requisition, authenticatedUserInfo.Id, cancellationToken);
            return new CreateRequisitionResult(createdRequisition);
        }
    }
}
