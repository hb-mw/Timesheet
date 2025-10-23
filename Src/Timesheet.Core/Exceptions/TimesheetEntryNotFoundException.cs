using System.Net;

namespace Timesheet.Core.Exceptions;

public class TimesheetEntryDoesNotExistException : TimesheetException
{
    public TimesheetEntryDoesNotExistException(Guid entryId)
        : base(
            $"Timesheet entry with ID '{entryId}' Does not exist.",
            "Timesheet.EntryDoesNotExist",
            HttpStatusCode.BadRequest,
            new Dictionary<string, object?> { ["EntryId"] = entryId })
    {
        EntryId = entryId;
    }

    public Guid EntryId { get; }
}