using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Interfaces.Services.Auth;
using FluentValidation;

namespace ChequeRequisiontService.Endpoints.Auth.Login;

public record LoginCommand(string UserNameOrEmail, string Password) : ICommand<LoginResponse>;

public record LoginResponse(string Token);

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.UserNameOrEmail).NotEmpty().WithMessage("Username or Email is required.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
    }
}

public class LoginHandler(IAuthenticationServices authenticationServices) : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly IAuthenticationServices _authenticationServices = authenticationServices;
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var token = await _authenticationServices.LoginAsync(request.UserNameOrEmail, request.Password);
        return new LoginResponse(token);
    }
}
