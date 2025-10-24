using System.Net;
using Timesheet.Core.Interfaces.Services;
using Timesheet.Tests.Controllers.Fixtures;

namespace Timesheet.Tests.Controllers.Unit;

[Collection("MapsterConfig")]
public partial class TimesheetControllerTests
{
    [Fact]
    public void GetWeek_ReturnsOk_WithEntries()
    {
        // Arrange
        var service = Substitute.For<ITimesheetEntryService>();
        var controller = new TimesheetController(service);

        var start = SampleData.ThisMonday;
        var entries = SampleData.EntriesForWeek(
            3,
            start,
            (100, 4m, 0, "Mon work"),
            (100, 4m, 1, "Tue work"),
            (200, 2m, 2, "Wed work"));

        service.GetEntriesForUserWeek(3, start).Returns(entries);

        var query = new GetWeekQuery(3, start);

        // Act
        var result = controller.GetWeek(query);

        // Assert
        var ok = result.Result.As<OkObjectResult>();
        ok.StatusCode.Should().Be((int)HttpStatusCode.OK);

        var response = ok.Value.As<IEnumerable<TimesheetEntryResponse>>().ToList();
        response.Should().HaveCount(3);
        response.Select(r => r.UserId).Distinct().Should().Equal(3);
        response.Select(r => r.ProjectId).Should().Contain(new[] { 100, 200 });

        service.Received(1).GetEntriesForUserWeek(3, start);
    }

    [Fact]
    public void GetWeek_ReturnsOk_EmptyList_WhenNoEntries()
    {
        // Arrange
        var service = Substitute.For<ITimesheetEntryService>();
        var controller = new TimesheetController(service);

        var start = SampleData.NextMonday;
        service.GetEntriesForUserWeek(5, start).Returns(new List<TimesheetEntry>());

        var query = new GetWeekQuery(5, start);

        // Act
        var result = controller.GetWeek(query);

        // Assert
        var ok = result.Result.As<OkObjectResult>();
        ok.StatusCode.Should().Be((int)HttpStatusCode.OK);

        var response = ok.Value.As<IEnumerable<TimesheetEntryResponse>>();
        response.Should().BeEmpty();

        service.Received(1).GetEntriesForUserWeek(5, start);
    }

    [Fact]
    public void GetWeek_WhenServiceThrows_BubblesUp()
    {
        // Arrange
        var service = Substitute.For<ITimesheetEntryService>();
        var controller = new TimesheetController(service);

        var start = SampleData.FarPast;
        service.GetEntriesForUserWeek(Arg.Any<int>(), Arg.Any<DateOnly>())
            .Returns(_ => throw new InvalidOperationException("boom"));

        var query = new GetWeekQuery(9, start);

        // Act
        var act = () => controller.GetWeek(query);

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("*boom*");
        service.Received(1).GetEntriesForUserWeek(9, start);
    }
    
    [Fact]
    public void GetTotals_ReturnsOk_WithAggregatedTotals()
    {
        // Arrange
        var service = Substitute.For<ITimesheetEntryService>();
        var controller = new TimesheetController(service);

        var start = SampleData.ThisMonday;

        // The service returns Dictionary<int, decimal>
        var totals = new Dictionary<int, decimal>
        {
            [100] = 8m,
            [200] = 4.5m
        };

        service.GetTotalHoursPerProject(7, start).Returns(totals);

        var query = new GetTotalPerProjectQuery(7, start);

        // Act
        var result = controller.GetTotals(query);

        // Assert
        var ok = result.Result.As<OkObjectResult>();
        ok.StatusCode.Should().Be((int)HttpStatusCode.OK);

        var response = ok.Value.As<IEnumerable<TotalHoursPerProjectResponse>>().ToList();
        response.Should().HaveCount(2);
        response.Should().ContainEquivalentOf(new TotalHoursPerProjectResponse(100, 8m));
        response.Should().ContainEquivalentOf(new TotalHoursPerProjectResponse(200, 4.5m));

        service.Received(1).GetTotalHoursPerProject(7, start);
    }

    [Fact]
    public void GetTotals_ReturnsOk_Empty_WhenNoProjects()
    {
        // Arrange
        var service = Substitute.For<ITimesheetEntryService>();
        var controller = new TimesheetController(service);

        var start = SampleData.FarFuture;
        service.GetTotalHoursPerProject(11, start).Returns(new Dictionary<int, decimal>());

        var query = new GetTotalPerProjectQuery(11, start);

        // Act
        var result = controller.GetTotals(query);

        // Assert
        var ok = result.Result.As<OkObjectResult>();
        ok.StatusCode.Should().Be((int)HttpStatusCode.OK);

        var response = ok.Value.As<IEnumerable<TotalHoursPerProjectResponse>>();
        response.Should().BeEmpty();

        service.Received(1).GetTotalHoursPerProject(11, start);
    }

    [Fact]
    public void GetTotals_WhenServiceThrows_BubblesUp()
    {
        // Arrange
        var service = Substitute.For<ITimesheetEntryService>();
        var controller = new TimesheetController(service);

        var start = SampleData.ThisMonday;
        service.GetTotalHoursPerProject(Arg.Any<int>(), Arg.Any<DateOnly>())
            .Returns(_ => throw new Exception("totals failed"));

        var query = new GetTotalPerProjectQuery(17, start);

        // Act
        var act = () => controller.GetTotals(query);

        // Assert
        act.Should().Throw<Exception>().WithMessage("*totals failed*");
        service.Received(1).GetTotalHoursPerProject(17, start);
    }
}