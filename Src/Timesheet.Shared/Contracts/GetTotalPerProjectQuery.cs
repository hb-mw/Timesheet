namespace Timesheet.Shared.Contracts;

public record GetTotalPerProjectQuery(int UserId, DateOnly StartDate);