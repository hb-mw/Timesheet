namespace Timesheet.Tests.Services;

public class TimesheetEntryServiceTests
{
    private readonly ITimesheetEntryRepository _repo;
    private readonly TimesheetEntryService _service;

    public TimesheetEntryServiceTests()
    {
        _repo = Substitute.For<ITimesheetEntryRepository>();
        _service = new TimesheetEntryService(_repo);
    }

    private static TimesheetEntry CreateEntry(
        int userId = 1,
        int projectId = 1,
        decimal hours = 8,
        DateOnly? date = null)
    {
        return new TimesheetEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProjectId = projectId,
            Date = date ?? DateOnly.FromDateTime(DateTime.Today),
            Hours = hours
        };
    }
    
    [Fact]
    public void AddEntry_ShouldAdd_WhenValid()
    {
        var entry = CreateEntry();

        _repo.GetForUserBetween(entry.UserId, entry.Date, entry.Date).Returns(Array.Empty<TimesheetEntry>());

        _service.AddEntry(entry);

        _repo.Received(1).Add(entry);
    }

    [Fact]
    public void AddEntry_ShouldThrow_WhenTotalDailyHoursExceeded()
    {
        var entry = CreateEntry(hours: 8);
        var existing = new List<TimesheetEntry> { CreateEntry(entry.UserId, hours: 6, date: entry.Date) };

        _repo.GetForUserBetween(entry.UserId, entry.Date, entry.Date).Returns(existing);

        var action = () => _service.AddEntry(entry);

        action.Should().Throw<TimesheetDailyHoursExceededException>()
            .WithMessage($"*{entry.Date}*");
    }

    [Fact]
    public void AddEntry_ShouldThrow_WhenDuplicateExists()
    {
        var entry = CreateEntry();
        var duplicate = CreateEntry(entry.UserId, entry.ProjectId, date: entry.Date, hours: 0);

        _repo.GetForUserBetween(entry.UserId, entry.Date, entry.Date).Returns(new[] { duplicate });

        var action = () => _service.AddEntry(entry);

        action.Should().Throw<TimesheetDuplicateEntryException>();
    }
    
    [Fact]
    public void UpdateEntry_ShouldThrow_WhenEntryDoesNotExist()
    {
        var entry = CreateEntry();
        _repo.Get(entry.Id).Returns((TimesheetEntry?)null);

        var action = () => _service.UpdateEntry(entry);

        action.Should().Throw<TimesheetEntryDoesNotExistException>();
    }

    [Fact]
    public void UpdateEntry_ShouldUpdate_WhenValid()
    {
        var existing = CreateEntry();
        var entry = CreateEntry(existing.UserId, existing.ProjectId, 10, existing.Date);
        entry.Id = existing.Id;

        _repo.Get(entry.Id).Returns(existing);
        _repo.GetForUserBetween(entry.UserId, entry.Date, entry.Date).Returns(new[] { existing });

        _service.UpdateEntry(entry);

        _repo.Received(1).Update(entry);
    }
    
    [Fact]
    public void DeleteEntry_ShouldThrow_WhenEntryDoesNotExist()
    {
        var id = Guid.NewGuid();
        _repo.Get(id).Returns((TimesheetEntry?)null);

        var action = () => _service.DeleteEntry(id);

        action.Should().Throw<TimesheetEntryDoesNotExistException>();
    }

    [Fact]
    public void DeleteEntry_ShouldDelete_WhenExists()
    {
        var id = Guid.NewGuid();
        var existing = CreateEntry();
        existing.Id = id;
        _repo.Get(id).Returns(existing);

        _service.DeleteEntry(id);

        _repo.Received(1).Delete(id);
    }
    
    [Fact]
    public void GetEntriesForUser_ShouldReturnRepositoryResults()
    {
        var entries = new List<TimesheetEntry> { CreateEntry() };
        _repo.GetForUser(1).Returns(entries);

        var result = _service.GetEntriesForUser(1);

        result.Should().BeEquivalentTo(entries);
    }
    
    [Fact]
    public void GetEntriesForUserWeek_ShouldCallRepositoryWithCorrectRange()
    {
        var userId = 1;
        var start = new DateOnly(2024, 1, 1);
        var expectedEnd = start.AddDays(6);

        _repo.GetForUserBetween(userId, start, expectedEnd).Returns(Array.Empty<TimesheetEntry>());

        var result = _service.GetEntriesForUserWeek(userId, start);

        _repo.Received(1).GetForUserBetween(userId, start, expectedEnd);
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetTotalHoursPerProject_ShouldAggregateCorrectly()
    {
        var userId = 1;
        var start = new DateOnly(2024, 1, 1);
        var entries = new List<TimesheetEntry>
        {
            CreateEntry(userId, 1, 5, start),
            CreateEntry(userId, 1, 3, start.AddDays(1)),
            CreateEntry(userId, 2, 4, start)
        };

        _repo.GetForUserBetween(userId, start, start.AddDays(6)).Returns(entries);

        var result = _service.GetTotalHoursPerProject(userId, start);

        result.Should().ContainKey(1).WhoseValue.Should().Be(8);
        result.Should().ContainKey(2).WhoseValue.Should().Be(4);
    }
}
