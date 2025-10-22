using Microsoft.AspNetCore.Mvc;
using Timesheet.Core.Interfaces.Services;
using Timesheet.Core.Models;

namespace Timesheet.API.Controllers;


public class TimesheetController(ITimesheetEntryService timesheetEntryService) : ApiController
{
    private readonly ITimesheetEntryService _timesheetEntryService = timesheetEntryService;

    [HttpGet("user/{userId:int}")]
    public ActionResult<IEnumerable<TimesheetEntry>> GetForUser(
        [FromRoute] int userId)
    {
        var entries = _timesheetEntryService.GetEntriesForUser(userId);
        return Ok(entries);
    }
    
    [HttpPost]
    public ActionResult<TimesheetEntry> Add([FromBody] TimesheetEntry entry)
    {
        _timesheetEntryService.AddEntry(entry);
        return Created();
    }
    
    [HttpPut("{id:guid}")]
    public ActionResult<TimesheetEntry> Update([FromRoute] Guid id, [FromBody] TimesheetEntry entry)
    {
        // ensure route id wins
        entry.Id = id;
        _timesheetEntryService.UpdateEntry(entry);
        return Ok(entry);
    }
    
    [HttpDelete("{id:guid}")]
    public IActionResult Delete([FromRoute] Guid id)
    {
        _timesheetEntryService.DeleteEntry(id);
        return NoContent();
    }
    
    [HttpGet("user/{userId:int}/week/{weekStart}")]
    public ActionResult<IEnumerable<TimesheetEntry>> GetWeek(
        [FromRoute] int userId,
        [FromRoute] string weekStart)
    {
        if (!DateOnly.TryParse(weekStart, out var start))
            return BadRequest("weekStart must be a valid date.");
        
        var entries = _timesheetEntryService.GetEntriesForUserWeek(userId, start);
        return Ok(entries);
    }
    
    [HttpGet("user/{userId:int}/week/{weekStart}/totals")]
    public ActionResult<Dictionary<int, decimal>> GetTotals(
        [FromRoute] int userId,
        [FromRoute] string weekStart)
    {
        if (!DateOnly.TryParse(weekStart, out var start))
            return BadRequest("weekStart must be a valid date (yyyy-MM-dd).");

        var totals = _timesheetEntryService.GetTotalHoursPerProject(userId, start);
        return Ok(totals);
    }
}