using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.User;
using ChequeRequisiontService.Core.Dto.UserMenuPermission;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.Models.CRDB;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace ChequeRequisiontService.Endpoints.User.CreateUser;

public record CreateUserCommand(int? BankId,int? BranchId,int? VendorId,string Name, string Email, string UserName, string PasswordHash, string ImagePath, int Role, string? IsActive=null):ICommand<CreateUserResult>;

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
    }
}

public class CreateUserHandler(
    IUserMenuPermissionRepo userMenuPermissionRepo,
    IDefaultMenuPermisionRepo defaultMenuPermisionRepo,
    IPasswordHasher<UserDto> passwordHasher,
    IUserRepo userRepo,
    AuthenticatedUserInfo authenticatedUse
) : ICommandHandler<CreateUserCommand, CreateUserResult>
{
    private readonly IUserRepo _userRepo = userRepo;
    private readonly IDefaultMenuPermisionRepo _defaultMenuPermisionRepo = defaultMenuPermisionRepo;
    private readonly IPasswordHasher<UserDto> _passwordHasher = passwordHasher;
    private readonly IUserMenuPermissionRepo _userMenuPermissionRepo = userMenuPermissionRepo;

    public async Task<CreateUserResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        bool activeStatus = request.IsActive != "InActive";
        request = request with { IsActive = null };

        var user = request.Adapt<UserDto>();
        user.IsActive = activeStatus;
        user.PasswordHash = _passwordHasher.HashPassword(user, request.PasswordHash);

        
        using var transaction = await _userRepo.BeginTransactionAsync(cancellationToken); 

        try
        {
            var defaultMenus = await _defaultMenuPermisionRepo.GetAllAsync(request.Role, cancellationToken);

            var createdUser = await _userRepo.CreateAsync(user, authenticatedUse.Id, cancellationToken);

            foreach (var menu in defaultMenus)
            {

                var perMenu = menu.Adapt<UserMenuPermissionDto>();
                perMenu.Id = 0;
                perMenu.UserId = createdUser.Id;

                await _userMenuPermissionRepo.CreateAsync(perMenu, authenticatedUse.Id, cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);

            return new CreateUserResult(createdUser);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

}
