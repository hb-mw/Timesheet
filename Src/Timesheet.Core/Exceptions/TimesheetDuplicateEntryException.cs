using System.Net;

namespace Timesheet.Core.Exceptions;

public class TimesheetDuplicateEntryException : TimesheetException
{
    public TimesheetDuplicateEntryException(int userId, int projectId, DateOnly date)
        : base(
            message: "A timesheet entry with the same user, project, and date already exists.",
            errorCode: "Timesheet.DuplicateEntry",
            statusCode: HttpStatusCode.BadRequest,
            details: new Dictionary<string, object?>
            {
                ["UserId"] = userId,
                ["ProjectId"] = projectId,
                ["Date"] = date.ToString("yyyy-MM-dd")
            })
    {
    }
}