using Microsoft.AspNetCore.Http.HttpResults;
using Timesheet.Core.Interfaces.Services;
using Timesheet.Tests.Controllers.Fixtures;

namespace Timesheet.Tests.Controllers.Unit;

public partial class TimesheetControllerTests
{
    [Fact]
    public void Add_ReturnsCreatedAtAction_WithResponse()
    {
        // Arrange
        var service = Substitute.For<ITimesheetEntryService>();
        var controller = new TimesheetController(service);

        var req = SampleData.UpsertRequest(
            5, 123, hours: 7.5m, date: SampleData.ThisMonday, description: "Initial impl");

        // Act
        var result = controller.Add(req);
        result.Should().BeOfType<CreatedResult>();
        
        // Service called once with correctly mapped domain entity
        service.Received(1).AddEntry(Arg.Is<TimesheetEntry>(e =>
            e.UserId == req.UserId &&
            e.ProjectId == req.ProjectId &&
            e.Hours == req.Hours &&
            e.Date == req.Date &&
            e.Description == req.Description
        ));
    }

    [Fact]
    public void Add_WhenServiceThrowsDuplicate_BubblesUp()
    {
        // Arrange
        var service = Substitute.For<ITimesheetEntryService>();
        service
            .When(s => s.AddEntry(Arg.Any<TimesheetEntry>()))
            .Do(_ => throw new TimesheetDuplicateEntryException(7, 777, SampleData.ThisMonday));

        var controller = new TimesheetController(service);
        var req = SampleData.UpsertRequest(7, 777, hours: 4m, date: SampleData.ThisMonday);

        // Act
        var action = () => controller.Add(req);

        // Assert (controller doesn't swallow it; middleware will map later)
        action.Should().Throw<TimesheetDuplicateEntryException>();

        service.Received(1).AddEntry(Arg.Any<TimesheetEntry>());
    }
    
    [Fact]
    public void Add_CallsService_WithMappedDomainEntry()
    {
        // Arrange
        var service = Substitute.For<ITimesheetEntryService>();
        var controller = new TimesheetController(service);

        var req = SampleData.UpsertRequest(9, 900, hours: 0m, date: SampleData.ThisMonday,
            description: "0 hours");

        // Act
        _ = controller.Add(req);

        // Assert: verify exact mapping that goes into the service
        service.Received(1).AddEntry(Arg.Is<TimesheetEntry>(e =>
            e.UserId == 9 &&
            e.ProjectId == 900 &&
            e.Hours == 0m &&
            e.Date == SampleData.ThisMonday &&
            e.Description == "0 hours"
        ));
    }
}