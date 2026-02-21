using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.DTOs;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace CleanArchitecture.Api.Middleware;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, exception.Message);

        var (statusCode, title, errors) = exception switch
        {
            NotFoundException => (
                HttpStatusCode.NotFound,
                "Resource Not Found",
                null),

            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                "Validation Failed",
                validationEx.Errors),

            BadRequestException => (
                HttpStatusCode.BadRequest,
                "Bad Request",
                null),

            UnauthorizedException => (
                HttpStatusCode.Unauthorized,
                "Unauthorized Access",
                null),

            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                "Access Denied",
                null),

            BadHttpRequestException => (
                HttpStatusCode.BadRequest,
                "Bad Request",
                null),

            _ => (
                HttpStatusCode.InternalServerError,
                "Internal Server Error",
                null)
        };

        var response = new ErrorResponse
        {
            StatusCode = (int)statusCode,
            Title = title,
            Message = environment.IsDevelopment()
                ? exception.Message
                : "An unexpected error occurred.",
            Errors = errors
        };

        httpContext.Response.StatusCode = response.StatusCode;

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}