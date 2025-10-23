using System.Net;

namespace Timesheet.Core.Exceptions;

public class TimesheetDuplicateEntryException : TimesheetException
{
    public TimesheetDuplicateEntryException(int userId, int projectId, DateOnly date)
        : base(
            "A timesheet entry with the same user, project, and date already exists.",
            "Timesheet.DuplicateEntry",
            HttpStatusCode.BadRequest,
            new Dictionary<string, object?>
            {
                ["UserId"] = userId,
                ["ProjectId"] = projectId,
                ["Date"] = date.ToString("yyyy-MM-dd")
            })
    {
    }
}