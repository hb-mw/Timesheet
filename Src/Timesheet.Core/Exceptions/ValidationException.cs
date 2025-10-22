using System.Net;

namespace Timesheet.Core.Exceptions;

public class ValidationException : TimesheetException
{
    public IReadOnlyDictionary<string, string[]> FieldErrors { get; }

    public ValidationException(
        IDictionary<string, string[]> fieldErrors,
        string message = "One or more validation errors occurred.")
        : base(
            message: message,
            errorCode: "Timesheet.ValidationFailed",
            statusCode: HttpStatusCode.BadRequest)
    {
        FieldErrors = new Dictionary<string, string[]>(fieldErrors);
    }

    public static ValidationException Single(string field, string message) =>
        new ValidationException(new Dictionary<string, string[]>
        {
            [field] = new[] { message }
        });
}