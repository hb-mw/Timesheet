using Mapster;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Core.Interfaces.Services;
using Timesheet.Core.Models;
using Timesheet.Shared.Contracts;

namespace Timesheet.API.Controllers;

public class TimesheetController(ITimesheetEntryService timesheetEntryService) : ApiController
{
    private readonly ITimesheetEntryService _timesheetEntryService = timesheetEntryService;

    // ----------------------------------------------------
    // GET all entries for a given user
    // ----------------------------------------------------
    [HttpGet("user/{userId:int}")]
    public ActionResult<IEnumerable<TimesheetEntryResponse>> GetForUser([FromRoute] int userId)
    {
        var entries = _timesheetEntryService.GetEntriesForUser(userId);
        var response = entries.Adapt<IEnumerable<TimesheetEntryResponse>>();
        return Ok(response);
    }

    // ----------------------------------------------------
    // POST - create a new entry
    // ----------------------------------------------------
    [HttpPost]
    public ActionResult<TimesheetEntryResponse> Add([FromBody] UpsertTimesheetEntryRequest request)
    {
        var entry = request.Adapt<TimesheetEntry>();
        _timesheetEntryService.AddEntry(entry);

        var response = entry.Adapt<TimesheetEntryResponse>();
        return CreatedAtAction(nameof(GetForUser), new { userId = response.UserId }, response);
    }

    // ----------------------------------------------------
    // PUT - update an existing entry
    // ----------------------------------------------------
    [HttpPut("{id:guid}")]
    public ActionResult<TimesheetEntryResponse> Update([FromRoute] Guid id,
        [FromBody] UpsertTimesheetEntryRequest request)
    {
        var entry = request.Adapt<TimesheetEntry>();
        entry.Id = id;
        _timesheetEntryService.UpdateEntry(entry);

        var response = entry.Adapt<TimesheetEntryResponse>();
        return Ok(response);
    }

    // ----------------------------------------------------
    // DELETE - delete entry
    // ----------------------------------------------------
    [HttpDelete("{id:guid}")]
    public IActionResult Delete([FromRoute] Guid id)
    {
        _timesheetEntryService.DeleteEntry(id);
        return NoContent();
    }

    // ----------------------------------------------------
    // GET entries for a user and week
    // ----------------------------------------------------
    [HttpGet("week")]
    public ActionResult<IEnumerable<TimesheetEntryResponse>> GetWeek([FromQuery] GetWeekQuery query)
    {
        var entries = _timesheetEntryService.GetEntriesForUserWeek(query.UserId, query.StartDate);
        var response = entries.Adapt<IEnumerable<TimesheetEntryResponse>>();
        return Ok(response);
    }

    // ----------------------------------------------------
    // GET total hours per project for a user and week
    // ----------------------------------------------------
    [HttpGet("project-totals")]
    public ActionResult<IEnumerable<TotalHoursPerProjectResponse>> GetTotals(
        [FromQuery] GetTotalPerProjectQuery query)
    {
        var totals = _timesheetEntryService.GetTotalHoursPerProject(query.UserId, query.StartDate);
        var response = totals.Adapt<IEnumerable<TotalHoursPerProjectResponse>>();
        return Ok(response);
    }
}