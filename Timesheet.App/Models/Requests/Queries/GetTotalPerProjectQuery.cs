namespace Timesheet.App.Models.Requests.Queries;

public record GetTotalPerProjectQuery(int UserId, DateOnly StartDate);