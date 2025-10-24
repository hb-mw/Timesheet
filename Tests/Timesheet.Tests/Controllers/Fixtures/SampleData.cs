namespace Timesheet.Tests.Controllers.Fixtures;

public class SampleData
{
    public static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
    public static readonly DateOnly ThisMonday = MondayOf(Today);
    public static readonly DateOnly NextMonday = ThisMonday.AddDays(7);
    public static readonly DateOnly FarPast = new(2015, 01, 05);
    public static readonly DateOnly FarFuture = new(2035, 12, 31);

    public static TimesheetEntry Entry(
        int userId = 1,
        int projectId = 100,
        DateOnly? date = null,
        decimal hours = 8m,
        string? description = "Worked on feature X",
        Guid? id = null)
    {
        return new TimesheetEntry
        {
            Id = id ?? Guid.NewGuid(),
            UserId = userId,
            ProjectId = projectId,
            Date = date ?? Today,
            Hours = hours,
            Description = description
        };
    }

    public static List<TimesheetEntry> OneEntryList(
        int userId = 1,
        int projectId = 100,
        DateOnly? date = null,
        decimal hours = 8m,
        string? description = "Worked on feature X")
    {
        return new List<TimesheetEntry> { Entry(userId, projectId, date, hours, description) };
    }

    public static List<TimesheetEntry> EntriesForWeek(
        int userId,
        DateOnly weekStart,
        params (int ProjectId, decimal Hours, int DayOffset, string? Description)[] items)
    {
        var list = new List<TimesheetEntry>();
        foreach (var (projectId, hrs, offset, desc) in items)
            list.Add(Entry(
                userId,
                projectId,
                weekStart.AddDays(offset),
                hrs,
                desc ?? $"Work D{offset}"));
        return list;
    }

    public static UpsertTimesheetEntryRequest UpsertRequest(
        int userId = 1,
        int projectId = 100,
        DateOnly? date = null,
        decimal hours = 8m,
        string? description = "Worked on feature X")
    {
        return new UpsertTimesheetEntryRequest(userId, projectId, date ?? Today, hours, description);
    }

    public static IEnumerable<KeyValuePair<int, decimal>> Totals(params (int ProjectId, decimal Hours)[] items)
    {
        return items.Select(x => new KeyValuePair<int, decimal>(x.ProjectId, x.Hours));
    }

    public static IEnumerable<KeyValuePair<int, decimal>> SimpleTotals()
    {
        return Totals((100, 8m), (200, 4.5m));
    }

    public static DateOnly MondayOf(DateOnly d)
    {
        return d.AddDays(-(int)d.DayOfWeek + (int)DayOfWeek.Monday);
    }
}