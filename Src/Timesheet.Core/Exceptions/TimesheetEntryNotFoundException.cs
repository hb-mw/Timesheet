using System.Net;

namespace Timesheet.Core.Exceptions;

public class TimesheetEntryDoesNotExistException : TimesheetException
{
    public Guid EntryId { get; }

    public TimesheetEntryDoesNotExistException(Guid entryId)
        : base(
            message: $"Timesheet entry with ID '{entryId}' Does not exist.",
            errorCode: "Timesheet.EntryDoesNotExist",
            statusCode: HttpStatusCode.BadRequest,
            details: new Dictionary<string, object?> { ["EntryId"] = entryId })
    {
        EntryId = entryId;
    }
}