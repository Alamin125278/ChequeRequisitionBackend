using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;

namespace ChequeRequisiontService.Endpoints.Vendor.DeleteVendor
{
    public record DeleteVendorCommand(int Id) : ICommand<DeleteVendorResult>;
    public record DeleteVendorResult(bool Success, string Message);
    public class DeleteVendorCommandValidator : AbstractValidator<DeleteVendorCommand>
    {
        public DeleteVendorCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Vendor ID must be greater than 0.");
        }
    }
    public class DeleteVendorHandler(IVendorRepo vendorRepo) : ICommandHandler<DeleteVendorCommand, DeleteVendorResult>
    {
        public async Task<DeleteVendorResult> Handle(DeleteVendorCommand request, CancellationToken cancellationToken)
        {
            var vendor = await vendorRepo.GetByIdAsync(request.Id, cancellationToken);
            var id = request.Id;
            if (vendor == null)
            {
                return new DeleteVendorResult(false, $"Vendor with ID {request.Id} not found.");
            }
            await vendorRepo.DeleteAsync(id,1, cancellationToken);
            return new DeleteVendorResult(true, $"Vendor with ID {request.Id} deleted successfully.");
        }
    }
}
