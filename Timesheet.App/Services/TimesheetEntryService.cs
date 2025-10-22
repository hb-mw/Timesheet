using Timesheet.App.Models.Requests;
using Timesheet.Core.Exceptions;
using Timesheet.Core.Interfaces.Repositories;
using Timesheet.Core.Interfaces.Services;
using Timesheet.Core.Models;

namespace Timesheet.App.Services;

// TODO : All Validations will be handled correctly later.
public class TimesheetEntryService(ITimesheetEntryRepository repository) : ITimesheetEntryService
{
    public void AddEntry(TimesheetEntry entry)
    {
        EnsureTotalDailyHoursNotExceeded(entry.UserId, entry.Date, entry.Hours);
        EnsureNotDuplicated(entry.UserId, entry.ProjectId, entry.Date);
        repository.Add(entry);
    }

    public void UpdateEntry(TimesheetEntry entry)
    {
        EnsureEntryExists(entry.Id);
        EnsureTotalDailyHoursNotExceeded(entry.UserId, entry.Date, entry.Hours);
        EnsureNotDuplicated(entry.UserId, entry.ProjectId, entry.Date);
        repository.Update(entry);
    }

    public void DeleteEntry(Guid id)
    {
        EnsureEntryExists(id);
        repository.Delete(id);
    }

    public IEnumerable<TimesheetEntry> GetEntriesForUser(int userId)
    {
        return repository.GetForUser(userId);
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

    private void EnsureEntryExists(Guid id)
    {
        if (!repository.Exists(id))
        {
            throw new TimesheetEntryDoesNotExistException(id);
        }
    }

    private void EnsureNotDuplicated(int userId, int projectId, DateOnly date)
    {
        if (repository.Exists(userId, projectId, date))
        {
            throw new TimesheetDuplicateEntryException(userId, projectId, date);
        }
    }

    private void EnsureTotalDailyHoursNotExceeded(int userId, DateOnly date, decimal hoursToAdd)
    {
        var entries = repository.GetForUserBetween(userId, date, date);
        var currentTotalHours = entries.Sum(e => e.Hours);
        if (currentTotalHours + hoursToAdd > 12)
        {
            throw new TimesheetDailyHoursExceededException(12 - currentTotalHours, date);
        }
    }
}