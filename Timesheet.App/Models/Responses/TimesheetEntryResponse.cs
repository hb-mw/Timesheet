namespace Timesheet.App.Models.Responses;

public record TimesheetEntryResponse(
    Guid Id,
    int UserId,
    int ProjectId,
    DateOnly Date,
    decimal Hours,
    string? Description
);