using FluentValidationException = FluentValidation.ValidationException;
using InventoryService.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Api.Errors;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        return exception switch
        {
            FluentValidationException validationException =>
                await HandleValidationExceptionAsync(httpContext, validationException),

            DuplicateSkuException duplicateSkuException =>
                await HandleDuplicateSkuExceptionAsync(httpContext, duplicateSkuException),

            ArgumentException argumentException =>
                await HandleBadRequestExceptionAsync(httpContext, argumentException),

            _ =>
                await HandleUnexpectedExceptionAsync(httpContext, exception)
        };
    }

    private async ValueTask<bool> HandleValidationExceptionAsync(
        HttpContext httpContext,
        FluentValidationException exception)
    {
        logger.LogWarning(exception, "Request validation failed.");

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        var errors = exception.Errors
            .GroupBy(error =>
                string.IsNullOrWhiteSpace(error.PropertyName)
                    ? "request"
                    : error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(error => error.ErrorMessage)
                    .Distinct()
                    .ToArray());

        var problemDetails = new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation failed.",
            Detail = "One or more validation errors occurred.",
            Instance = httpContext.Request.Path
        };

        return await WriteProblemDetailsAsync(httpContext, exception, problemDetails);
    }

    private async ValueTask<bool> HandleDuplicateSkuExceptionAsync(
        HttpContext httpContext,
        DuplicateSkuException exception)
    {
        logger.LogWarning(exception, "Duplicate product SKU.");

        httpContext.Response.StatusCode = StatusCodes.Status409Conflict;

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Duplicate SKU.",
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        return await WriteProblemDetailsAsync(httpContext, exception, problemDetails);
    }

    private async ValueTask<bool> HandleBadRequestExceptionAsync(
        HttpContext httpContext,
        Exception exception)
    {
        logger.LogWarning(exception, "Bad request.");

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Bad request.",
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        return await WriteProblemDetailsAsync(httpContext, exception, problemDetails);
    }

    private async ValueTask<bool> HandleUnexpectedExceptionAsync(
        HttpContext httpContext,
        Exception exception)
    {
        logger.LogError(exception, "An unexpected error occurred.");

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Detail = "The server encountered an unexpected error.",
            Instance = httpContext.Request.Path
        };

        return await WriteProblemDetailsAsync(httpContext, exception, problemDetails);
    }

    private async ValueTask<bool> WriteProblemDetailsAsync(
        HttpContext httpContext,
        Exception exception,
        ProblemDetails problemDetails)
    {
        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        var wasWritten = await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        });

        if (wasWritten)
        {
            return true;
        }

        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken: httpContext.RequestAborted);

        return true;
    }
}