using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.User;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace ChequeRequisiontService.Endpoints.User.CreateUser;

public record CreateUserCommand(int? BankId,int? BranchId,int? VendorId,string Name, string Email, string UserName, string PasswordHash, string ImagePath, string Role, bool IsActive):ICommand<CreateUserResult>;

public record CreateUserResult(UserDto User);

public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Valid email is required.");
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required.");
        RuleFor(x => x.PasswordHash).NotEmpty().WithMessage("Password is required.");
        RuleFor(x => x.ImagePath).NotEmpty().WithMessage("Image path is required.");
        RuleFor(x => x.Role).NotEmpty().WithMessage("Role is required.");
        RuleFor(x => x.IsActive).NotNull().WithMessage("IsActive is required.");
    }
}

public class CreateUserHandler(IPasswordHasher<UserDto> passwordHasher, IUserRepo userRepo):ICommandHandler<CreateUserCommand, CreateUserResult>
{
    private readonly IUserRepo _userRepo = userRepo;
    private readonly IPasswordHasher<UserDto> _passwordHasher = passwordHasher;
    public async Task<CreateUserResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = request.Adapt<UserDto>();
        user.PasswordHash= _passwordHasher.HashPassword(user, request.PasswordHash);

        var createdUser = await _userRepo.CreateAsync(user, 1);
        
        return new CreateUserResult(createdUser);
    }
}
