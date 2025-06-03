using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.User;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace ChequeRequisiontService.Endpoints.Auth.ChangedPass;

public record ChangePasswordCommand(int Id, string CurrentPassword, string NewPassword)
    : ICommand<ChangePasswordResult>;

public record ChangePasswordResult(bool IsSuccess, string Message);

public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6);
    }
}

public class ChangedPassHandler(
    AuthenticatedUserInfo authenticatedUserInfo,
    IUserRepo userRepo,
    IPasswordHasher<UserDto> passwordHasher
) : ICommandHandler<ChangePasswordCommand, ChangePasswordResult>
{
    private readonly IUserRepo _userRepo = userRepo;
    private readonly IPasswordHasher<UserDto> _passwordHasher = passwordHasher;
    private readonly AuthenticatedUserInfo _authenticatedUserInfo = authenticatedUserInfo;
    public async Task<ChangePasswordResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepo.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
        {
            return new ChangePasswordResult(false, "User not found.");
        }
        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return new ChangePasswordResult(false, "Current password is incorrect.");
        }

       var newPasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
        var updatedUser = new ChangedPasswordDto
        {
            Id = user.Id,
            NewPassword = newPasswordHash
        };

        var result = await _userRepo.UpdatedPasswordAsunc(request.Id,updatedUser, _authenticatedUserInfo.Id, cancellationToken);
        return result
            ? new ChangePasswordResult(true, "Password changed successfully.")
            : new ChangePasswordResult(false, "Failed to change password.");
    }
}
