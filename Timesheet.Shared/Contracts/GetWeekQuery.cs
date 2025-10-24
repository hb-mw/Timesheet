namespace Timesheet.Shared.Contracts;

public record GetWeekQuery(int UserId, DateOnly StartDate);