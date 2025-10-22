namespace Timesheet.App.Models.Requests;

public record UpsertTimesheetEntryRequest(
    int UserId,
    int ProjectId,
    DateOnly? Date,
    decimal Hours,
    string? Description
);