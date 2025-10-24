using System.Net;
using Timesheet.Core.Interfaces.Services;
using Timesheet.Tests.Controllers.Fixtures;

namespace Timesheet.Tests.Controllers.Unit;

public partial class TimesheetControllerTests
{
    [Fact]
    public void Update_ReturnsOk_WithMappedResponse()
    {
        // Arrange
        var service = Substitute.For<ITimesheetEntryService>();
        var controller = new TimesheetController(service);

        var id = Guid.NewGuid();
        var request = SampleData.UpsertRequest(
            2, 222, hours: 6.5m, date: SampleData.ThisMonday, description: "Refactor module");

        // Act
        var result = controller.Update(id, request);

        // Assert
         result.Should().BeOfType<OkResult>();
             
        service.Received(1).UpdateEntry(Arg.Is<TimesheetEntry>(e =>
            e.Id == id &&
            e.UserId == request.UserId &&
            e.ProjectId == request.ProjectId &&
            e.Hours == request.Hours &&
            e.Date == request.Date &&
            e.Description == request.Description
        ));
    }

    [Fact]
    public void Update_WhenEntryDoesNotExist_BubblesUp()
    {
        // Arrange
        var service = Substitute.For<ITimesheetEntryService>();
        var controller = new TimesheetController(service);

        var id = Guid.NewGuid();
        var request = SampleData.UpsertRequest(9, 900, hours: 3m, date: SampleData.ThisMonday);

        service
            .When(s => s.UpdateEntry(Arg.Any<TimesheetEntry>()))
            .Do(_ => throw new TimesheetEntryDoesNotExistException(id));

        // Act
        var act = () => controller.Update(id, request);

        // Assert
        act.Should().Throw<TimesheetEntryDoesNotExistException>();
        service.Received(1).UpdateEntry(Arg.Is<TimesheetEntry>(e => e.Id == id));
    }

    [Fact]
    public void Update_MapsRouteIdOntoDomainEntity()
    {
        // Arrange
        var service = Substitute.For<ITimesheetEntryService>();
        var controller = new TimesheetController(service);

        var id = Guid.NewGuid();
        var request = SampleData.UpsertRequest(
            5, 555, hours: 0m, date: SampleData.ThisMonday, description: "Zero hours edge");

        // Act
        _ = controller.Update(id, request);

        // Assert
        service.Received(1).UpdateEntry(Arg.Is<TimesheetEntry>(e =>
            e.Id == id &&
            e.UserId == 5 &&
            e.ProjectId == 555 &&
            e.Hours == 0m &&
            e.Date == SampleData.ThisMonday &&
            e.Description == "Zero hours edge"
        ));
    }
}