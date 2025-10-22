using System.Collections.Concurrent;
using Timesheet.Core.Interfaces.Repositories;
using Timesheet.Core.Models;

namespace Timesheet.Infra.Data.Repositories;

public class InMemoryTimesheetEntryRepository : ITimesheetEntryRepository
{
    private ConcurrentDictionary<Guid, TimesheetEntry> _entries = [];

    public void Add(TimesheetEntry entry)
    {
        _entries.TryAdd(entry.Id, entry);
    }

    public void Update(TimesheetEntry entry)
    {
        _entries[entry.Id] = entry;
    }

    public void Delete(Guid id)
    {
        _entries.TryRemove(id, out _);
    }

    public IEnumerable<TimesheetEntry> GetForUser(int userId)
    {
        return _entries.Values
            .Where(e => e.UserId == userId)
            .OrderBy(e => e.Date)
            .ToList();
    }

    public IEnumerable<TimesheetEntry> GetForUserBetween(int userId, DateOnly from, DateOnly to)
    {
        return _entries.Values
            .Where(e => e.UserId == userId && e.Date >= from && e.Date <= to)
            .OrderBy(e => e.Date)
            .ToList();
    }

    public bool Exists(int userId, int projectId, DateOnly date)
    {
        return _entries.Values.Any(e =>
            e.UserId == userId &&
            e.ProjectId == projectId &&
            e.Date == date);
    }

    public bool Exists(Guid id)
    {
        return _entries.ContainsKey(id);
    }
}