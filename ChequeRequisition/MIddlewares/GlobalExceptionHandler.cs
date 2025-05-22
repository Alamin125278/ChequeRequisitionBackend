using BuildingBlocks.Exceptions;
using ChequeRequisiontService.Core.Dto.Common;
using Microsoft.AspNetCore.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace ChequeRequisiontService.MIddlewares;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError("Error Message: {exceptionMessage}, Time of occurrence {time}", exception.Message, DateTime.UtcNow);

        int statusCode = exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
			ChequeRequisiontService.Exceptions.NotFoundException => StatusCodes.Status404NotFound,
			ValidationException => StatusCodes.Status400BadRequest,
			ChequeRequisiontService.Exceptions.ValidationException => StatusCodes.Status400BadRequest,
			BadRequestException => StatusCodes.Status400BadRequest,
            ChequeRequisiontService.Exceptions.UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
			ChequeRequisiontService.Exceptions.ForbiddenException => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        var response = new ResponseDto<string>
        {
            Success = false,
            StatusCode = statusCode,
            Message = exception.Message,
            Data = null
        };
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response), cancellationToken: cancellationToken);
        return true;
    }
}
