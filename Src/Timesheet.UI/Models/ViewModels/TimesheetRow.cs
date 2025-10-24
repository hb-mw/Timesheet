using Timesheet.Shared.Contracts;

namespace Timesheet.UI.Models.ViewModels;

public class TimesheetRow
{
    public Guid Id { get; set; }
    public int UserId { get; set; }
    public int ProjectId { get; set; }
    public Dictionary<DateOnly, TimesheetEntryResponse> Entries { get; set; } = new();
    public decimal TotalHours => Entries.Values.Sum(x => x.Hours);
}