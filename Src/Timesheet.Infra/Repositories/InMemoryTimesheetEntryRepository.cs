using Timesheet.Core.Interfaces.Repositories;
using Timesheet.Core.Models;

namespace Timesheet.Infra.Repositories;

public class InMemoryTimesheetEntryRepository : ITimesheetEntryRepository
{
    private readonly List<TimesheetEntry> _entries = [];
    
    public void Add(TimesheetEntry entry)
    {
        _entries.Add(entry);
    }

    public void Update(TimesheetEntry entry)
    {
        var index = _entries.FindIndex(e => e.Id == entry.Id);
        
        if (index != -1)
        {
            _entries[ index ] = entry;
        }
        else
        {
            //TODO Better exception handling and should reflect the responses in the API
            throw new KeyNotFoundException("Entry not found on update");
        }
    }

    public void Delete(Guid id)
    {
        var entryToDelete = _entries.FirstOrDefault(entry => entry.Id == id);
        if (entryToDelete is null)
        {
            //TODO Better exception handling and should reflect the responses in the API
            throw new KeyNotFoundException("Entry not found on delete.");
        }
    }

    public TimesheetEntry? GetById(Guid id)
    {
        // TODO: to be handled better to return 404 in our api.
        return _entries.FirstOrDefault(entry => entry.Id == id);
    }

    public IEnumerable<TimesheetEntry> GetForUserBetween(int userId, DateOnly from, DateOnly to)
    {
        return _entries
            .Where(e => e.UserId == userId && e.Date >= from && e.Date <= to)
            .OrderBy(e => e.Date)
            .ToList();
    }

    public bool Exists(int userId, int projectId, DateOnly date)
    {
        // TODO : maybe find a better solution , lets get core functionality working first.
        return _entries.Any(e => e.UserId == userId && e.ProjectId == projectId && e.Date == date);
    }
}