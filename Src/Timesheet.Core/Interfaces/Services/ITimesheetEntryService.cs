using Timesheet.Core.Models;

namespace Timesheet.Core.Interfaces.Services;

public interface ITimesheetEntryService
{
    void AddEntry(TimesheetEntry entry);
    void UpdateEntry(TimesheetEntry entry);
    void DeleteEntry(Guid id);
    
    IEnumerable<TimesheetEntry> GetEntriesForUser(int userId);
    
    IEnumerable<TimesheetEntry> GetEntriesForUserWeek(int userId, DateOnly weekStart);
    Dictionary<int, decimal> GetTotalHoursPerProject(int userId, DateOnly weekStart);
}