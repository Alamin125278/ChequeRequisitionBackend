using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;
using Mapster;

namespace ChequeRequisiontService.Endpoints.Vendor.UpdateVendor
{
    public record UpdateVendorCommand(int Id, string VendorName, string Email, string Phone,string Address, string? PhotoPath, bool IsActive) : ICommand<UpdateVendorResult>;
   public record UpdateVendorResult(Core.Dto.Vendor.VendorDto Vendor);

    public class UpdateVendorValidator : AbstractValidator<UpdateVendorCommand>
    {
        public UpdateVendorValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
            RuleFor(x => x.VendorName).NotEmpty().WithMessage("Vendor name is required.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Valid email is required.");
            RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone number is required.");
            RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required.");
            RuleFor(x => x.IsActive).NotNull().WithMessage("IsActive is required.");
        }
    }
    public class UpdateVendorHandler(IVendorRepo vendorRepo) : ICommandHandler<UpdateVendorCommand, UpdateVendorResult>
    {
        private readonly IVendorRepo _vendorRepo = vendorRepo;
        public async Task<UpdateVendorResult> Handle(UpdateVendorCommand request, CancellationToken cancellationToken)
        {
            var vendor = request.Adapt<Core.Dto.Vendor.VendorDto>();
            var id = request.Id;
            
            var updatedVendor = await _vendorRepo.UpdateAsync(vendor,id, 1, cancellationToken);
            
            return new UpdateVendorResult(updatedVendor);
        }
    }
}
