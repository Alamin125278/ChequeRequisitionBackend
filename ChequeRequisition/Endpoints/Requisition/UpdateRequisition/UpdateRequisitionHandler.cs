using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Requisition;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;
using Mapster;
using System.Windows.Input;

namespace ChequeRequisiontService.Endpoints.Requisition.UpdateRequisition
{
    public record UpdateRequisitionCommand(int Id,int BankId,int BranchId, int AccountNo, int RoutingNo, int StartNo, int EndNo, string ChequeType, string ChequePrefix, string MicrNo, string Series, string AccountName, string CusAddress, int BookQty, int TransactionCode, int Leaves, int CourierCode, int ReceivingBranchId, int Serverity) : ICommand<UpdateRequisitionResult>;
    public record UpdateRequisitionResult(string Message, RequisitionDto Requisition);
    public class UpdateRequisitionCommandValidator : AbstractValidator<UpdateRequisitionCommand>
    {
        public UpdateRequisitionCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
            RuleFor(x => x.BankId).NotEmpty().WithMessage("Bank Id is required.");
            RuleFor(x => x.BranchId).NotEmpty().WithMessage("Branch Id is required.");
            RuleFor(x => x.AccountNo).NotEmpty().WithMessage("Account number is required.");
            RuleFor(x => x.RoutingNo).NotEmpty().WithMessage("Routing number is required.");
            RuleFor(x => x.StartNo).GreaterThan(0).WithMessage("Start number must be greater than 0.");
            RuleFor(x => x.EndNo).GreaterThan(0).WithMessage("End number must be greater than 0.");
            RuleFor(x => x.ChequeType).NotEmpty().WithMessage("Cheque type is required.");
            RuleFor(x => x.ChequePrefix).NotEmpty().WithMessage("Cheque prefix is required.");
            RuleFor(x => x.MicrNo).NotEmpty().WithMessage("MICR number is required.");
            RuleFor(x => x.Series).NotEmpty().WithMessage("Series is required.");
            RuleFor(x => x.AccountName).NotEmpty().WithMessage("Account name is required.");
            RuleFor(x => x.CusAddress).NotEmpty().WithMessage("Customer address is required.");
            RuleFor(x => x.BookQty).GreaterThan(0).WithMessage("Book quantity must be greater than 0.");
            RuleFor(x => x.TransactionCode).GreaterThan(0).WithMessage("Transaction code must be greater than 0.");
            RuleFor(x => x.Leaves).GreaterThan(0).WithMessage("Leaves must be greater than 0.");
            RuleFor(x => x.CourierCode).GreaterThan(0).WithMessage("Courier code must be greater than 0.");
            RuleFor(x => x.ReceivingBranchId).GreaterThan(0).WithMessage("Receiving branch ID must be greater than 0.");
        }
    }
    public class UpdateRequisitionHandler(IRequisitonRepo requisitonRepo) : ICommandHandler<UpdateRequisitionCommand, UpdateRequisitionResult>
    {
        private readonly IRequisitonRepo _requisitonRepo = requisitonRepo;
        public async Task<UpdateRequisitionResult> Handle(UpdateRequisitionCommand request, CancellationToken cancellationToken)
        {
            var requisition = request.Adapt<RequisitionDto>();
            var id = request.Id;
            //requisition.RequestDate = DateOnly.Parse(request.RequestDate);

            var updatedRequisition = await _requisitonRepo.UpdateAsync(requisition, id, 1, cancellationToken);
            
            return new UpdateRequisitionResult("Cheque requisition updated successfully", updatedRequisition);
        }
    }
}
