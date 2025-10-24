using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using Timesheet.Shared.Contracts;

namespace Timesheet.UI.Pages;

/// <summary>
///     Represents the page that displays a summary of total hours per project
///     for a specific user and week period.
/// </summary>
public partial class UserProjects : ComponentBase
{
    /// <summary>
    ///     Tracks whether data is currently being loaded.
    /// </summary>
    private bool _loading;

    /// <summary>
    ///     Holds the total recorded hours per project for the selected user and week.
    /// </summary>
    private List<TotalHoursPerProjectResponse>? _totals;

    /// <summary>
    ///     The ID of the user whose project hours are being displayed.
    /// </summary>
    private int? UserId { get; set; } = 1;

    /// <summary>
    ///     The starting date of the selected week.
    ///     Defaults to the current week's Monday.
    /// </summary>
    private DateTime WeekStart { get; set; } = DateTime.Today.StartOfWeek(DayOfWeek.Monday);

    /// <summary>
    ///     Indicates whether the displayed week should always start on a Monday.
    /// </summary>
    private bool EnforceMonday { get; set; } = true;

    /// <summary>
    ///     Lifecycle method that runs when the component is first initialized.
    ///     Loads the initial project-hour data.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }

    /// <summary>
    ///     Handles updates when the filter bar changes.
    ///     Updates the user, week, and enforcement values and reloads the data.
    /// </summary>
    /// <param name="filter">Tuple containing the new filter parameters (UserId, WeekStart, EnforceMonday).</param>
    private async Task OnFilterChanged((int? UserId, DateTime WeekStart, bool EnforceMonday) filter)
    {
        UserId = filter.UserId;
        WeekStart = filter.WeekStart;
        EnforceMonday = filter.EnforceMonday;
        await LoadDataAsync();
    }

    /// <summary>
    ///     Loads and refreshes the total hours per project for the current user and week.
    ///     Displays error notifications if the operation fails.
    /// </summary>
    private async Task LoadDataAsync()
    {
        if (UserId is null or <= 0)
        {
            _totals = [];
            return;
        }

        _loading = true;
        var result = await TimesheetAppService.GetTotalProjectsForUser(UserId.Value, WeekStart);

        if (result.Success)
        {
            _totals = result.Data;
            _loading = false;
            return;
        }
        
        Snackbar.Add(result.Errors[0]??"Unknown error!", Severity.Error);
        _loading = false;
    }
}