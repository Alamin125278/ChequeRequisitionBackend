using ChequeRequisiontService.Core.Dto.Auth;
using ChequeRequisiontService.Core.Dto.User;
using ChequeRequisiontService.Core.Interfaces.Repositories;
using System.IdentityModel.Tokens.Jwt;

namespace ChequeRequisiontService.MIddlewares;

public class AuthenticationMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context, AuthenticatedUserInfo authenticatedUserInfo, IUserRepo userRepo)
    {
        var authorizationHeader = context.Request.Headers["Authorization"].ToString();
        if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer ")) 
        {
            var token = authorizationHeader.Substring("Bearer ".Length);

            try
            {
                var handler = new JwtSecurityTokenHandler();
                if (handler.CanReadToken(token)) 
                { 
                    var jwtToken = handler.ReadJwtToken(token);
                    var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                    if (userId == null) {
                        throw new UnauthorizedAccessException("UserId claim not found in token.");
                    }
                    var user = await userRepo.GetByIdAsync(int.Parse(userId));
                    if (user==null || user.IsActive==false)
                    {
                       throw new UnauthorizedAccessException("User not found or inactive.");
                    }
                    authenticatedUserInfo.Id = user.Id;
                    authenticatedUserInfo.Name = user.Name;
                    authenticatedUserInfo.Email = user.Email;
                    authenticatedUserInfo.UserName = user.UserName;
                    authenticatedUserInfo.ImagePath = user.ImagePath;
                    authenticatedUserInfo.Role = user.Role;
                    authenticatedUserInfo.IsActive = user.IsActive;
                    authenticatedUserInfo.BankId = user.BankId;
                    authenticatedUserInfo.BranchId = user.BranchId;
                    authenticatedUserInfo.VendorId = user.VendorId;

                }
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException("Invalid token.", ex);
            }
        }

        await _next(context);
    }
}
