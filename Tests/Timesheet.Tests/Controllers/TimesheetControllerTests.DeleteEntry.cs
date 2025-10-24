using System.Net;
using Timesheet.Core.Interfaces.Services;

namespace Timesheet.Tests.Controllers.Unit;

public partial class TimesheetControllerTests
{
    [Fact]
    public void Delete_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        var service = Substitute.For<ITimesheetEntryService>();
        var controller = new TimesheetController(service);
        var id = Guid.NewGuid();

        // Act
        var result = controller.Delete(id);

        // Assert
        var noContent = result.As<NoContentResult>();
        noContent.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

        service.Received(1).DeleteEntry(id);
    }

    [Fact]
    public void Delete_WhenEntryDoesNotExist_BubblesUp()
    {
        // Arrange
        var service = Substitute.For<ITimesheetEntryService>();
        var controller = new TimesheetController(service);
        var id = Guid.NewGuid();

        service
            .When(s => s.DeleteEntry(id))
            .Do(_ => throw new TimesheetEntryDoesNotExistException(id));

        // Act
        var act = () => controller.Delete(id);

        // Assert
        act.Should().Throw<TimesheetEntryDoesNotExistException>();
        service.Received(1).DeleteEntry(id);
    }

    [Fact]
    public void Delete_AllowsGuidEmpty_EvenThoughRouteWouldNormallyFilter()
    {
        // Arrange
        var service = Substitute.For<ITimesheetEntryService>();
        var controller = new TimesheetController(service);
        var id = Guid.Empty;

        // Act
        var result = controller.Delete(id);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        service.Received(1).DeleteEntry(id);
    }
}