using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Timesheet.Core.Exceptions;
using FluentValidationValidationException = FluentValidation.ValidationException;
using TimesheetValidationException = Timesheet.Core.Exceptions.ValidationException;

namespace Timesheet.API.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (TimesheetValidationException ex)
        {
            _logger.LogInformation(
                "Validation failed {Code}. Fields={Count} TraceId={TraceId} Details={@Details}",
                ex.ErrorCode, ex.FieldErrors.Count, context.TraceIdentifier, ex.FieldErrors);

            await WriteProblemAsync(context,
                StatusCodes.Status400BadRequest,
                ex.Message,
                ex.ErrorCode,
                new Dictionary<string, object?> { ["errors"] = ex.FieldErrors });
        }
        catch (FluentValidationValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            _logger.LogInformation(
                "FluentValidation failed. Fields={Count} TraceId={TraceId} Errors={@Errors}",
                errors.Count, context.TraceIdentifier, errors);

            await WriteProblemAsync(context,
                StatusCodes.Status400BadRequest,
                "Validation failed.",
                "Timesheet.ValidationFailed",
                new Dictionary<string, object?> { ["errors"] = errors });
        }
        catch (TimesheetException ex)
        {
            _logger.LogInformation(
                "Domain error {Code} ({Status}). TraceId={TraceId}. Message='{Message}'. Details={@Details}",
                ex.ErrorCode, ex.StatusCode, context.TraceIdentifier, ex.Message, ex.Details);

            await WriteProblemAsync(context,
                ex.StatusCode,
                ex.Message,
                ex.ErrorCode,
                ex.Details);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception. TraceId={TraceId}", context.TraceIdentifier);

            await WriteProblemAsync(context,
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.",
                "Server.Error",
                null);
        }
    }

    private async Task WriteProblemAsync(
        HttpContext context,
        int statusCode,
        string title,
        string code,
        IReadOnlyDictionary<string, object?>? details)
    {
        if (context.Response.HasStarted)
        {
            _logger.LogWarning("Response already started. Cannot write problem. TraceId={TraceId}",
                context.TraceIdentifier);
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json; charset=utf-8";

        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        var problem = new Dictionary<string, object?>
        {
            ["type"] = $"https://httpstatuses.com/{statusCode}",
            ["title"] = title,
            ["status"] = statusCode,
            ["code"] = code,
            ["traceId"] = traceId
        };

        if (details is { Count: > 0 })
            problem["details"] = details;

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOptions));
    }
}