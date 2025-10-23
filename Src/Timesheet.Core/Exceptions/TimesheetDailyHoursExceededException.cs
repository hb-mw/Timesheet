using System.Net;

namespace Timesheet.Core.Exceptions;

public class TimesheetDailyHoursExceededException : TimesheetException
{
    public TimesheetDailyHoursExceededException(decimal hoursLeft, DateOnly date)
        : base(
            $"A single day can only have a maximum of 12 hours of work." +
            $" You have only {hoursLeft} hours left for the day {date}.",
            "Timesheet.EntryDoesNotExist",
            HttpStatusCode.BadRequest,
            new Dictionary<string, object?>())
    {
    }
}