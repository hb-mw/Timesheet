namespace Timesheet.Core.Exceptions;

public class ValidationException : TimesheetException
{
    public ValidationException(
        IDictionary<string, string[]> fieldErrors,
        string message = "One or more validation errors occurred.")
        : base(
            message,
            "Timesheet.ValidationFailed")
    {
        FieldErrors = new Dictionary<string, string[]>(fieldErrors);
    }

    public IReadOnlyDictionary<string, string[]> FieldErrors { get; }

    public static ValidationException Single(string field, string message)
    {
        return new ValidationException(new Dictionary<string, string[]>
        {
            [field] = new[] { message }
        });
    }
}