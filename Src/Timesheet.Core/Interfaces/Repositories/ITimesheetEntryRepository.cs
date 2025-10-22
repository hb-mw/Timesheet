using Timesheet.Core.Models;

namespace Timesheet.Core.Interfaces.Repositories;

public interface ITimesheetEntryRepository
{
    void Add(TimesheetEntry entry);
    void Update(TimesheetEntry entry);
    void Delete(Guid id);
    TimesheetEntry? GetById(Guid id);

    IEnumerable<TimesheetEntry> GetForUserBetween(int userId, DateOnly from, DateOnly to);

    bool Exists(int userId, int projectId, DateOnly date);
}