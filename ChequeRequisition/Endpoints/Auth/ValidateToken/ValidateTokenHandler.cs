using BuildingBlocks.CQRS;
using ChequeRequisiontService.Core.Interfaces.Services.Auth;
using Microsoft.IdentityModel.Tokens;

namespace ChequeRequisiontService.Endpoints.Auth.ValidateToken;
public record ValidateTokenCommand(string Token) : ICommand<ValidateTokenResponse>;

public record ValidateTokenResponse(bool IsValid);

public class ValidateTokenHandler(IAuthenticationServices authenticationServices) : ICommandHandler<ValidateTokenCommand, ValidateTokenResponse>
{
    public async Task<ValidateTokenResponse> Handle(ValidateTokenCommand request, CancellationToken cancellationToken)
    {
        var isValid= await authenticationServices.ValidateTokenAsync(request.Token, cancellationToken);

        return new ValidateTokenResponse(isValid);

    }
}
