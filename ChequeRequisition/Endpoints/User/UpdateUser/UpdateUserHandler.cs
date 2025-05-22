using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.User;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;
using Mapster;

namespace ChequeRequisiontService.Endpoints.User.UpdateUser
{
    public record UpdateUserCommand(int Id, int? BankId, int? BranchId, int? VendorId, string Name, string Email, string UserName, string ImagePath, string Role, bool IsActive) : ICommand<UpdateUserResult>;
   public record UpdateUserResult(UserDto User);
    public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Valid email is required.");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required.");
            RuleFor(x => x.ImagePath).NotEmpty().WithMessage("Image path is required.");
            RuleFor(x => x.Role).NotEmpty().WithMessage("Role is required.");
            RuleFor(x => x.IsActive).NotNull().WithMessage("IsActive is required.");
        }
    }
    public class UpdateUserHandler(IUserRepo userRepo) : ICommandHandler<UpdateUserCommand, UpdateUserResult>
    {
        private readonly IUserRepo _userRepo = userRepo;
        public async Task<UpdateUserResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = request.Adapt<UserDto>();
            var id = request.Id;
            
            var updatedUser = await _userRepo.UpdateAsync(user,id, 1, cancellationToken);
            
            return new UpdateUserResult(updatedUser);
        }
    }
}
