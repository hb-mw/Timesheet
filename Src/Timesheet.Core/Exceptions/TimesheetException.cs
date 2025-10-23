using System.Net;

namespace Timesheet.Core.Exceptions;

public class TimesheetException : Exception
{
    protected TimesheetException(
        string message,
        string errorCode,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        IDictionary<string, object?>? details = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        StatusCode = (int)statusCode;
        Details = details is null
            ? new Dictionary<string, object?>()
            : new Dictionary<string, object?>(details);
    }

    /// <summary>
    ///     Machine-readable error code (stable for clients/logging).
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    ///     Suggested HTTP status code to map to at the API boundary.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    ///     Optional key/value details (e.g., { "EntryId": "...", "UserId": "1" }).
    /// </summary>
    public IReadOnlyDictionary<string, object?> Details { get; }
}