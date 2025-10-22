namespace Timesheet.Core.Models;

public class TimesheetEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public int UserId { get; set; }
    public int ProjectId { get; set; }
    public DateOnly Date { get; set; }
    public decimal Hours { get; set; }

    public string? Description { get; set; }
}