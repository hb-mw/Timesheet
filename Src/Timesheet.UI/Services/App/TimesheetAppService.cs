using System.Net;
using System.Net.Http.Json;
using Mapster;
using Timesheet.Shared.Contracts;
using Timesheet.UI.Models;
using Timesheet.UI.Models.Forms;
using Timesheet.UI.Models.ViewModels;
using Timesheet.UI.Services.Data;

namespace Timesheet.UI.Services.App;

/// <summary>
/// Provides application-level business logic for managing timesheet entries and related data.
/// Acts as an intermediary between the UI and the underlying <see cref="TimesheetDataService"/>.
/// </summary>
public class TimesheetAppService(TimesheetDataService dataService)
{
    /// <summary>
    /// Creates a new timesheet entry by sending a request to the data service.
    /// </summary>
    /// <param name="model">The form model containing entry details.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> containing the created entry or validation errors if the operation fails.
    /// </returns>
    public async Task<ServiceResult<TimesheetEntryResponse>> AddEntry(TimesheetEntryFormModel model)
    {
        var request = model.Adapt<UpsertTimesheetEntryRequest>();
        var response = await dataService.AddEntry(request);

        if (response.IsSuccessStatusCode)
        {
            var created = await response.Content.ReadFromJsonAsync<TimesheetEntryResponse>();
            return ServiceResult<TimesheetEntryResponse>.Ok(created!, "Entry created successfully.");
        }

        return await HandleApiError<TimesheetEntryResponse>(response);
    }

    /// <summary>
    /// Retrieves all timesheet entries for a specific user and week.
    /// </summary>
    /// <param name="userId">The user ID whose entries are to be fetched.</param>
    /// <param name="weekStart">The start date of the week (typically Monday).</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> containing a list of <see cref="TimesheetEntryResponse"/> objects.
    /// </returns>
    public async Task<ServiceResult<List<TimesheetEntryResponse>>> GetWeekForUserId(int userId, DateTime weekStart)
    {
        var query = new GetWeekQuery(userId, DateOnly.FromDateTime(weekStart));
        var response = await dataService.GetWeekAsync(query);

        if (response.IsSuccessStatusCode)
        {
            var entries = await response.Content.ReadFromJsonAsync<List<TimesheetEntryResponse>>();
            return ServiceResult<List<TimesheetEntryResponse>>.Ok(entries, "Entries fetched successfully.");
        }

        return await HandleApiError<List<TimesheetEntryResponse>>(response);
    }

    /// <summary>
    /// Updates an existing timesheet entry.
    /// </summary>
    /// <param name="model">The updated entry data from the form.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> indicating success or failure.
    /// </returns>
    public async Task<ServiceResult<bool>> UpdateEntry(TimesheetEntryFormModel model)
    {
        var request = model.Adapt<UpsertTimesheetEntryRequest>();
        var response = await dataService.UpdateEntryAsync(model.Id?.ToString(), request);

        if (response.IsSuccessStatusCode)
            return ServiceResult<bool>.Ok(true, "Entry updated successfully.");

        return await HandleApiError<bool>(response);
    }

    /// <summary>
    /// Deletes a timesheet entry by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entry to delete.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> indicating whether the deletion succeeded.
    /// </returns>
    public async Task<ServiceResult<bool>> DeleteEntry(Guid id)
    {
        var response = await dataService.DeleteEntryAsync(id.ToString());

        if (response.IsSuccessStatusCode)
            return ServiceResult<bool>.Ok(true, "Entry deleted successfully.");

        return await HandleApiError<bool>(response);
    }

    /// <summary>
    /// Retrieves total recorded hours per project for a specific user and week.
    /// </summary>
    /// <param name="userId">The ID of the user whose data is being retrieved.</param>
    /// <param name="weekStart">The starting date of the week.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> containing a list of <see cref="TotalHoursPerProjectResponse"/>.
    /// </returns>
    public async Task<ServiceResult<List<TotalHoursPerProjectResponse>>> GetTotalProjectsForUser(
        int userId,
        DateTime weekStart)
    {
        var query = new GetTotalPerProjectQuery(userId, DateOnly.FromDateTime(weekStart));
        var response = await dataService.GetTotalProjectsForUserAsync(query);

        if (response.IsSuccessStatusCode)
        {
            var entries = await response.Content.ReadFromJsonAsync<List<TotalHoursPerProjectResponse>>();
            return ServiceResult<List<TotalHoursPerProjectResponse>>.Ok(entries, "Project totals fetched successfully.");
        }

        return await HandleApiError<List<TotalHoursPerProjectResponse>>(response);
    }

    /// <summary>
    /// Handles common API error responses and maps them to standardized <see cref="ServiceResult{T}"/> objects.
    /// </summary>
    /// <typeparam name="T">The expected result type.</typeparam>
    /// <param name="response">The HTTP response from the API call.</param>
    /// <returns>A failed <see cref="ServiceResult{T}"/> with extracted error messages.</returns>
    private static async Task<ServiceResult<T>> HandleApiError<T>(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiError>();
            if (error?.Title?.Contains("validation errors", StringComparison.OrdinalIgnoreCase) == true)
            {
                var errors = error.Errors?.Select(e => e.Value.ToString()) ?? ["Bad request"];
                return ServiceResult<T>.Fail(errors);
            }

            return ServiceResult<T>.Fail([error?.Title ?? "Bad request"]);
        }

        return ServiceResult<T>.Fail($"Unexpected error: {response.StatusCode}");
    }

    /// <summary>
    /// Builds a list of <see cref="TimesheetRow"/> objects from raw timesheet entries,
    /// To be shown in the table view.
    /// </summary>
    /// <param name="allEntries">All timesheet entries retrieved from the API.</param>
    /// <param name="userId">The user ID for which to build rows.</param>
    /// <param name="weekStart">The week start date (typically Monday).</param>
    /// <returns>
    /// A list of <see cref="TimesheetRow"/> objects representing timesheet data structured for UI display.
    /// </returns>
    public static List<TimesheetRow> BuildRows(
        List<TimesheetEntryResponse>? allEntries,
        int userId,
        DateTime weekStart)
    {
        if (allEntries is null or { Count: 0 })
        {
            return [];
        }

        var weekDates = Enumerable.Range(0, 7)
            .Select(i => DateOnly.FromDateTime(weekStart.AddDays(i)))
            .ToList();

        var filtered = allEntries
            .Where(e => (e.UserId == userId)
                        && e.Date >= weekDates.First()
                        && e.Date <= weekDates.Last())
            .GroupBy(e => new { e.UserId, e.ProjectId })
            .Select(g => new TimesheetRow
            {
                Id = g.First().Id,
                UserId = g.Key.UserId,
                ProjectId = g.Key.ProjectId,
                Entries = g.ToDictionary(x => x.Date, x => x)
            })
            .ToList();

        return filtered;
    }
}
