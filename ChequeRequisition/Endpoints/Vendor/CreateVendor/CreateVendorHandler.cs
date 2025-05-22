using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Vendor;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;
using Mapster;
using System.Windows.Input;

namespace ChequeRequisiontService.Endpoints.Vendor.CreateVendor
{
    public record CreateVendorCommand(string VendorName, string Email, string Phone, string Address,string? PhotoPath,Boolean IsActive):ICommand<CreateVendorResult>;
    
    public record CreateVendorResult(VendorDto Vendor);
    public class CreateVendorValidator : AbstractValidator<CreateVendorCommand>
    {
        public CreateVendorValidator()
        {
            RuleFor(x => x.VendorName).NotEmpty().WithMessage("Vendor Name is required.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Valid email is required.");
            RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone number is required.");
            RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required.");
            RuleFor(x => x.IsActive).NotNull().WithMessage("IsActive is required.");
        }
    }
    public class CreateVendorHandler(IVendorRepo vendorRepo):ICommandHandler<CreateVendorCommand, CreateVendorResult>
    {
        private readonly IVendorRepo _vendorRepo = vendorRepo;
        public async Task<CreateVendorResult> Handle(CreateVendorCommand request, CancellationToken cancellationToken)
        {
            var vendor = request.Adapt<VendorDto>();
            var createdVendor = await _vendorRepo.CreateAsync(vendor, 1, cancellationToken);
            return new CreateVendorResult(createdVendor);
        }
    }
}
