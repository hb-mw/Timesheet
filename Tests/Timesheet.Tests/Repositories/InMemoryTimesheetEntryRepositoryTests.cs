namespace Timesheet.Tests.Repositories;

public class InMemoryTimesheetEntryRepositoryTests
{
    private readonly InMemoryTimesheetEntryRepository _repo;

    public InMemoryTimesheetEntryRepositoryTests()
    {
        _repo = new InMemoryTimesheetEntryRepository();
    }

    [Fact]
    public void Add_ShouldStoreEntry()
    {
        var entry = CreateEntry();

        _repo.Add(entry);

        var result = _repo.Get(entry.Id);
        Assert.NotNull(result);
    }

    [Fact]
    public void Update_ShouldModifyExistingEntry()
    {
        var entry = CreateEntry();
        _repo.Add(entry);

        entry.Hours = 10;
        _repo.Update(entry);

        var updated = _repo.GetForUser(entry.UserId).First();
        Assert.Equal(10, updated.Hours);
    }

    [Fact]
    public void Delete_ShouldRemoveEntry()
    {
        var entry = CreateEntry();
        _repo.Add(entry);

        _repo.Delete(entry.Id);

        var result = _repo.Get(entry.Id);
        Assert.Null(result);
    }

    [Fact]
    public void GetForUser_ShouldReturnOnlyThatUsersEntries()
    {
        var user1 = CreateEntry();
        var user2 = CreateEntry(2);
        _repo.Add(user1);
        _repo.Add(user2);

        var result = _repo.GetForUser(1);

        Assert.Single(result);
        Assert.All(result, e => Assert.Equal(1, e.UserId));
    }

    [Fact]
    public void GetForUserBetween_ShouldFilterByDateRange()
    {
        var userId = 1;
        _repo.Add(CreateEntry(userId, date: new DateOnly(2024, 1, 1)));
        _repo.Add(CreateEntry(userId, date: new DateOnly(2024, 1, 10)));
        _repo.Add(CreateEntry(userId, date: new DateOnly(2024, 1, 20)));

        var results = _repo.GetForUserBetween(userId, new DateOnly(2024, 1, 5), new DateOnly(2024, 1, 15));

        Assert.Single(results);
        Assert.Equal(new DateOnly(2024, 1, 10), results.First().Date);
    }


    [Fact]
    public void Add_ShouldNotOverwriteExistingEntry_WhenIdAlreadyExists()
    {
        var entry1 = CreateEntry();
        var entry2 = CreateEntry();
        entry2.Id = entry1.Id; // same ID different data

        _repo.Add(entry1);
        _repo.Add(entry2);

        var result = _repo.GetForUser(entry1.UserId).Single();

        Assert.Equal(entry1.Hours, result.Hours);
    }

    private static TimesheetEntry CreateEntry(int userId = 1, int projectId = 1, DateOnly? date = null)
    {
        return new TimesheetEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProjectId = projectId,
            Date = date ?? DateOnly.FromDateTime(DateTime.Today),
            Hours = 8
        };
    }
}