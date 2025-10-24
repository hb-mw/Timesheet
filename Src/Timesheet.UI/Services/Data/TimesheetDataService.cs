using System.Net.Http.Json;
using Timesheet.Shared.Contracts;

namespace Timesheet.UI.Services.Data;

/// <summary>
///     Provides data access methods for interacting with the Timesheet API.
///     Handles CRUD operations related to timesheet entries and project summaries.
/// </summary>
public class TimesheetDataService(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    /// <summary>
    ///     Sends a POST request to create a new timesheet entry.
    /// </summary>
    /// <param name="request">The entry data to create.</param>
    /// <returns>The HTTP response from the API.</returns>
    public async Task<HttpResponseMessage> AddEntry(UpsertTimesheetEntryRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(string.Empty, request);
        return response;
    }

    /// <summary>
    ///     Sends a GET request to retrieve all timesheet entries for a given user and week.
    /// </summary>
    /// <param name="query">The query parameters containing the user ID and week start date.</param>
    /// <returns>The HTTP response containing the list of entries for the week.</returns>
    public async Task<HttpResponseMessage> GetWeekAsync(GetWeekQuery query)
    {
        var url = $"week?userId={query.UserId}&startDate={query.StartDate:yyyy-MM-dd}";
        var response = await _httpClient.GetAsync(url);
        return response;
    }

    /// <summary>
    ///     Sends a PUT request to update an existing timesheet entry.
    /// </summary>
    /// <param name="id">The unique identifier of the entry to update.</param>
    /// <param name="request">The updated entry data.</param>
    /// <returns>The HTTP response from the API.</returns>
    public async Task<HttpResponseMessage> UpdateEntryAsync(string id, UpsertTimesheetEntryRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync(id, request);
        return response;
    }

    /// <summary>
    ///     Sends a DELETE request to remove a timesheet entry by its string identifier.
    ///     (Alternative version for string-based IDs.)
    /// </summary>
    /// <param name="id">The identifier of the entry to delete.</param>
    /// <returns>The HTTP response from the API.</returns>
    public async Task<HttpResponseMessage> DeleteEntryAsync(string id)
    {
        var response = await _httpClient.DeleteAsync(id);
        return response;
    }

    /// <summary>
    ///     Sends a GET request to retrieve total recorded hours per project
    ///     for a specific user and week.
    /// </summary>
    /// <param name="query">The query parameters containing user ID and week start date.</param>
    /// <returns>The HTTP response containing total hours per project.</returns>
    public async Task<HttpResponseMessage> GetTotalProjectsForUserAsync(GetTotalPerProjectQuery query)
    {
        var url = $"project-totals?userId={query.UserId}&startDate={query.StartDate:yyyy-MM-dd}";
        var response = await _httpClient.GetAsync(url);
        return response;
    }
}