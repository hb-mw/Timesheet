namespace Timesheet.App.Models.Requests.Queries;

public record GetWeekQuery(int UserId, DateOnly StartDate);