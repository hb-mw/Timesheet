using Timesheet.Core.Exceptions;
using Timesheet.Core.Interfaces.Repositories;
using Timesheet.Core.Interfaces.Services;
using Timesheet.Core.Models;

namespace Timesheet.App.Services;

public class TimesheetEntryService(ITimesheetEntryRepository repository) : ITimesheetEntryService
{
    public void AddEntry(TimesheetEntry entry)
    {
        EnsureTotalDailyHoursNotExceeded(entry, null);
        EnsureNotDuplicated(entry, null);
        repository.Add(entry);
    }

    public void UpdateEntry(TimesheetEntry entry)
    {
        var existing = EnsureEntryExists(entry.Id);

        EnsureTotalDailyHoursNotExceeded(entry, existing);
        EnsureNotDuplicated(entry, existing.Id);
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
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Hours));
    }

    private TimesheetEntry EnsureEntryExists(Guid id)
    {
        var existing = repository.Get(id);
        if (existing is null)
            throw new TimesheetEntryDoesNotExistException(id);

        return existing;
    }

    private void EnsureNotDuplicated(TimesheetEntry entry, Guid? excludeId)
    {
        var duplicates = repository
            .GetForUserBetween(entry.UserId, entry.Date, entry.Date)
            .Any(e => e.ProjectId == entry.ProjectId && e.Id != excludeId);

        if (duplicates)
            throw new TimesheetDuplicateEntryException(entry.UserId, entry.ProjectId, entry.Date);
    }

    private void EnsureTotalDailyHoursNotExceeded(TimesheetEntry entry, TimesheetEntry? existing)
    {
        var entries = repository
            .GetForUserBetween(entry.UserId, entry.Date, entry.Date)
            .ToList();

        var currentTotalHours = entries.Sum(e => e.Hours);

        if (existing is not null && existing.UserId == entry.UserId && existing.Date == entry.Date)
            currentTotalHours -= existing.Hours;

        if (currentTotalHours + entry.Hours > 12)
            throw new TimesheetDailyHoursExceededException(12 - currentTotalHours, entry.Date);
    }
}