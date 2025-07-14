using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.User;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;
using Mapster;

namespace ChequeRequisiontService.Endpoints.User.UpdateUser
{
    public record UpdateUserCommand(int Id, int? BankId, int? BranchId, int? VendorId, string Name, string Email, string UserName, int Role, string? ImagePath = null, string? IsActive = null) : ICommand<UpdateUserResult>;
   public record UpdateUserResult(UserDto User);
    public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Valid email is required.");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required.");
            RuleFor(x => x.Role).NotEmpty().WithMessage("Role is required.");
            RuleFor(x => x.IsActive).NotNull().WithMessage("IsActive is required.");
        }
    }
    public class UpdateUserHandler(IUserRepo userRepo,AuthenticatedUserInfo authenticatedUserInfo) : ICommandHandler<UpdateUserCommand, UpdateUserResult>
    {
        private readonly IUserRepo _userRepo = userRepo;
        public async Task<UpdateUserResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            bool activeStatus = request.IsActive != "InActive";
            request = request with { IsActive = null };
            var user = request.Adapt<UserDto>();
            user.IsActive = activeStatus;
            var id = request.Id;
            
            var updatedUser = await _userRepo.UpdateAsync(user,id,authenticatedUserInfo.Id, cancellationToken);
            
            return new UpdateUserResult(updatedUser);
        }
    }
}
