namespace Timesheet.Shared.Contracts;

public record UpsertTimesheetEntryRequest(
    int UserId,
    int ProjectId,
    DateOnly Date,
    decimal Hours,
    string? Description
);