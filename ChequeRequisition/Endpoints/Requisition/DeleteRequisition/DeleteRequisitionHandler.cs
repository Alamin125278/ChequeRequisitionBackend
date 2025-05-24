using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;

namespace ChequeRequisiontService.Endpoints.Requisition.DeleteRequisition
{
    public record DeleteRequisitionCommand(int Id) : ICommand<DeleteRequisitionResult>;

    public record DeleteRequisitionResult(bool IsDeleted, string Message);
    public class DeleteRequisitionCommandValidator : AbstractValidator<DeleteRequisitionCommand>
    {
        public DeleteRequisitionCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
        }
    }
    public class DeleteRequisitionHandler(IRequisitonRepo requisitionRepo) : ICommandHandler<DeleteRequisitionCommand, DeleteRequisitionResult>
    {
        public async Task<DeleteRequisitionResult> Handle(DeleteRequisitionCommand request, CancellationToken cancellationToken)
        {
            var result = await requisitionRepo.DeleteAsync(request.Id, 1, cancellationToken);
            return new DeleteRequisitionResult(result, result ? "Cheque Requisition Deleted Successfully" : "Cheque Requisition Not Found");
        }
    }
}
