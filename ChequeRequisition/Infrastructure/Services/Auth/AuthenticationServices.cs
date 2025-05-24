using ChequeRequisiontService.Core.Dto.User;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using ChequeRequisiontService.Core.Interfaces.Services.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChequeRequisiontService.Infrastructure.Services.Auth;

public class AuthenticationServices(IUserRepo userRepo, IPasswordHasher<UserDto> passwordHasher, IConfiguration configuration) : IAuthenticationServices
{
    private readonly IUserRepo _userRepo = userRepo;
    private readonly IPasswordHasher<UserDto> _passwordHasher = passwordHasher;
    private readonly IConfiguration _configuration = configuration;

    public async Task<string> LoginAsync(string UserNameOrEmail, string Password)
    {
        var handler = new JwtSecurityTokenHandler();
        string key = _configuration.GetSection("ApiSettings:Secret").Value ?? "";
        int expiryDays = int.Parse(_configuration.GetSection("ApiSettings:expireInDay").Value ?? "7");
        var data = await _userRepo.GetUserByEmailOrUserName(UserNameOrEmail);

        if(data == null || data.IsActive == false)
        {
            throw new NotFoundException("User not found or inactive");
        }

         var result = _passwordHasher.VerifyHashedPassword(data, data?.PasswordHash, Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Invalid password");
        }

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
            SecurityAlgorithms.HmacSha256Signature);

        var token = handler.CreateToken(new SecurityTokenDescriptor
        {
            Subject = GenerateClaims(data),
            Expires = DateTime.UtcNow.AddDays(expiryDays),
            SigningCredentials = credentials,
            Audience = _configuration.GetSection("ApiSettings:Audience")?.Value,
            Issuer = _configuration.GetSection("ApiSettings:Issuer")?.Value
        });

        return handler.WriteToken(token);

    }

    public Task<UserDto> LogoutAsync()
    {
        throw new NotImplementedException();
    }

    private static ClaimsIdentity GenerateClaims(UserDto user)
    {
        var claims = new ClaimsIdentity();
        claims.AddClaim(new Claim("UserId", user.Id.ToString()));
        
        return claims;
    }


}
