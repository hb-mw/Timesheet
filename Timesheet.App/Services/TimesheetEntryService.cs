using Timesheet.Core.Interfaces.Repositories;
using Timesheet.Core.Interfaces.Services;
using Timesheet.Core.Models;

namespace Timesheet.App.Services; 

// TODO : All Validations will be handled correctly later.
public class TimesheetEntryService(ITimesheetEntryRepository repository) : ITimesheetEntryService
{
    public void AddEntry(TimesheetEntry entry)
    {
        if (repository.Exists(entry.UserId, entry.ProjectId, entry.Date))
        {
            // TODO : Same all exceptions should be handled better. xD
            throw new InvalidOperationException("Entry already exists.!!");
        }
        repository.Add(entry);
    }

    public void UpdateEntry(TimesheetEntry entry)
    {
        repository.Update(entry);
    }

    public void DeleteEntry(Guid id)
    {
        repository.Delete(id);   
    }

    public IEnumerable<TimesheetEntry> GetEntriesForUserWeek(int userId, DateOnly startDate)
    {
        var result = repository.GetForUserBetween(userId, startDate, startDate.AddDays(6));
        return result;
    }

    public Dictionary<int, decimal> GetTotalHoursPerProject(int userId, DateOnly startDate)
    {
        var entries = GetEntriesForUserWeek(userId, startDate);
        return entries
            .GroupBy(e => e.ProjectId)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Hours));
    }
}