using Carter;
using ChequeRequisiontService.Core.Dto.Common;
using MediatR;

namespace ChequeRequisiontService.Endpoints.Auth.ValidateToken;

public class ValidateTokenEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/auth/validate-token", async (HttpContext httpContext,ISender sender) =>
        {
            var authHeader = httpContext.Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Results.Unauthorized();

            var token = authHeader["Bearer ".Length..].Trim();

            var command = new ValidateTokenCommand(token);
            var result = await sender.Send(command, CancellationToken.None);
            var response = new ResponseDto<ValidateTokenResponse>
            {
                Data = result,
                Success = result.IsValid,
                Message = result.IsValid ? "Token is valid." : "Token is invalid.",
                StatusCode= result.IsValid ? 200 : 401
            };
            return Results.Ok(response);
        })
          .Produces<ResponseDto<ValidateTokenResponse>>(200)
          .ProducesProblem(401)
          .RequireAuthorization()
          .WithName("ValidateToken")
          .WithTags("Auth");
    }
}
