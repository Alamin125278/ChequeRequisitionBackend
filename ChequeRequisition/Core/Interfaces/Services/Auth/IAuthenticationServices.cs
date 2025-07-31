using ChequeRequisiontService.Core.Dto.User;

namespace ChequeRequisiontService.Core.Interfaces.Services.Auth;

public interface IAuthenticationServices
{
    Task<string> LoginAsync(string UserNameOrEmail, string Password);
    Task<UserDto> LogoutAsync();
    Task<bool> ValidateTokenAsync(string token,CancellationToken cancellationToken=default);

}
