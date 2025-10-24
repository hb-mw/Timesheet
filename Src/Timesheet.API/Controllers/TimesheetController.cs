using Mapster;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Core.Interfaces.Services;
using Timesheet.Core.Models;
using Timesheet.Shared.Contracts;

namespace Timesheet.API.Controllers;

/// <summary>
/// Handles HTTP requests related to timesheet entries, including creation, updating,
/// deletion, and retrieval of weekly data and project totals.
/// </summary>
public class TimesheetController(ITimesheetEntryService timesheetEntryService) : ApiController
{
    /// <summary>
    /// Creates a new timesheet entry.
    /// </summary>
    /// <param name="request">The entry data to be created.</param>
    /// <returns>
    /// A <see cref="CreatedResult"/> response if the entry is successfully created.
    /// </returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult Add([FromBody] UpsertTimesheetEntryRequest request)
    {
        var entry = request.Adapt<TimesheetEntry>();
        timesheetEntryService.AddEntry(entry);
        return Created();
    }
    
    /// <summary>
    /// Updates an existing timesheet entry identified by its <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The unique identifier of the entry to update.</param>
    /// <param name="request">The updated entry data.</param>
    /// <returns>
    /// A <see cref="TimesheetEntryResponse"/> representing the updated entry.
    /// </returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult Update(
        [FromRoute] Guid id,
        [FromBody] UpsertTimesheetEntryRequest request)
    {
        var entry = request.Adapt<TimesheetEntry>();
        entry.Id = id;
        timesheetEntryService.UpdateEntry(entry);
        
        return Ok();
    }
    
    /// <summary>
    /// Deletes a timesheet entry by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entry to delete.</param>
    /// <returns>
    /// A <see cref="NoContentResult"/> response indicating the deletion was successful.
    /// </returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult Delete([FromRoute] Guid id)
    {
        timesheetEntryService.DeleteEntry(id);
        return NoContent();
    }
    
    /// <summary>
    /// Retrieves all timesheet entries for a specified user and week.
    /// </summary>
    /// <param name="query">
    /// The query parameters containing the user ID and the starting date of the week.
    /// </param>
    /// <returns>
    /// A collection of <see cref="TimesheetEntryResponse"/> objects representing the user's entries for the week.
    /// </returns>
    [HttpGet("week")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<IEnumerable<TimesheetEntryResponse>> GetWeek([FromQuery] GetWeekQuery query)
    {
        var entries = timesheetEntryService.GetEntriesForUserWeek(query.UserId, query.StartDate);
        var response = entries.Adapt<IEnumerable<TimesheetEntryResponse>>();
        return Ok(response);
    }
    
    /// <summary>
    /// Retrieves the total recorded hours per project for a specified user and week.
    /// </summary>
    /// <param name="query">
    /// The query parameters containing the user ID and the week start date.
    /// </param>
    /// <returns>
    /// A collection of <see cref="TotalHoursPerProjectResponse"/> objects representing total hours grouped by project.
    /// </returns>
    [HttpGet("project-totals")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<IEnumerable<TotalHoursPerProjectResponse>> GetTotals(
        [FromQuery] GetTotalPerProjectQuery query)
    {
        var totals = timesheetEntryService.GetTotalHoursPerProject(query.UserId, query.StartDate);
        var response = totals.Adapt<IEnumerable<TotalHoursPerProjectResponse>>();
        return Ok(response);
    }
}
