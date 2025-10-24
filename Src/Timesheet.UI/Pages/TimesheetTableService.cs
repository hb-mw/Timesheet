using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using Timesheet.Shared.Contracts;
using Timesheet.UI.Models.Forms;
using Timesheet.UI.Models.ViewModels;
using Timesheet.UI.Services.App;

namespace Timesheet.UI.Pages;

/// <summary>
/// Represents the main timesheet table component responsible for displaying,
/// filtering, and managing timesheet entries.
/// </summary>
public partial class TimesheetTable : ComponentBase
{
    /// <summary>
    /// Snackbar service for displaying notifications to the user.
    /// </summary>
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    /// <summary>
    /// The ID of the current user whose timesheet is being displayed.
    /// </summary>
    private int UserId { get; set; } = 1;

    /// <summary>
    /// The start date of the week currently being displayed.
    /// </summary>
    private DateTime WeekStart { get; set; } = DateTime.Today.StartOfWeek(DayOfWeek.Monday);

    /// <summary>
    /// Indicates whether to enforce that the selected week always starts on Monday.
    /// </summary>
    private bool EnforceMonday { get; set; } = true;

    /// <summary>
    /// A list of dates representing the days shown in the current week view.
    /// </summary>
    private List<DateOnly> DaysToShow { get; set; } = new();

    /// <summary>
    /// The list of timesheet rows currently filtered and displayed.
    /// </summary>
    private List<TimesheetRow> FilteredRows { get; set; } = new();

    /// <summary>
    /// Indicates whether data is currently being loaded.
    /// </summary>
    private bool IsLoading { get; set; }

    /// <summary>
    /// Tracks whether the form for adding or editing entries is currently open.
    /// </summary>
    private bool _isFormOpen;

    /// <summary>
    /// The form model bound to the add/edit entry dialog.
    /// </summary>
    private TimesheetEntryFormModel _formModel = new();

    /// <summary>
    /// Keeps track of expanded cells in the UI to manage row details display.
    /// </summary>
    private readonly HashSet<Guid> _expandedCells = []; 

    /// <summary>
    /// Called when the component is initialized.
    /// Fetches the initial timesheet data.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        await FetchFilteredData();
    }

    /// <summary>
    /// Loads timesheet entries for the current user and week and prepares the display data.
    /// </summary>
    private async Task FetchFilteredData()
    {
        IsLoading = true;

        if (EnforceMonday)
            WeekStart = WeekStart.StartOfWeek(DayOfWeek.Monday);

        DaysToShow = Enumerable.Range(0, 7)
            .Select(i => DateOnly.FromDateTime(WeekStart.AddDays(i)))
            .ToList();

        var entries = await TimesheetAppService.GetWeekForUserId(UserId, WeekStart);
        IsLoading = false;
        FilteredRows = TimesheetAppService.BuildRows(entries.Data, UserId, WeekStart);
    }

    /// <summary>
    /// Calculates the total hours recorded for a specific day.
    /// </summary>
    /// <param name="date">The date for which to calculate total hours.</param>
    /// <returns>The total number of hours for the given day.</returns>
    private decimal GetDailyTotal(DateOnly date)
        => FilteredRows.Sum(r => r.Entries.TryGetValue(date, out var e) ? e.Hours : 0);

    /// <summary>
    /// Handles submission of the add or edit form.
    /// Calls the appropriate service method and updates the UI accordingly.
    /// </summary>
    /// <param name="model">The submitted form model.</param>
    private async Task HandleFormSubmit(TimesheetEntryFormModel model)
    {
        if (model.Id is null)
        {
            var result = await TimesheetAppService.AddEntry(model);
            if (result.Success)
            {
                Snackbar.Add("Entry added.", Severity.Success);
                await FetchFilteredData();
                _isFormOpen = false;
                return;
            }

            Snackbar.Add(result.Errors[0], Severity.Error);
        }
        else
        {
            var result = await TimesheetAppService.UpdateEntry(model);
            if (result.Success)
            {
                Snackbar.Add("Entry Updated!", Severity.Success);
                await FetchFilteredData();
                _isFormOpen = false;
                return;
            }

            Snackbar.Add(result.Errors[0], Severity.Error);
        }
    }

    /// <summary>
    /// Opens the form pre-filled with an existing entry for editing.
    /// </summary>
    /// <param name="entry">The entry to edit.</param>
    private void OpenEditForm(TimesheetEntryResponse entry)
    {
        _formModel = new TimesheetEntryFormModel
        {
            Id = entry.Id,
            UserId = entry.UserId,
            ProjectId = entry.ProjectId,
            Date = DateTime.Parse(entry.Date.ToString()),
            Hours = entry.Hours,
            Description = entry.Description
        };
        _isFormOpen = true;
    }

    /// <summary>
    /// Deletes a specific timesheet entry and refreshes the displayed data.
    /// </summary>
    /// <param name="id">The unique identifier of the entry to delete.</param>
    private async Task DeleteEntry(Guid id)
    {
        var result = await TimesheetAppService.DeleteEntry(id);
        if (result.Success)
        {
            Snackbar.Add("Entry deleted.", Severity.Warning);
            await FetchFilteredData();
            return;
        }

        if (result.Errors.Count > 0)
        {
            Snackbar.Add(result.Errors[0], Severity.Error);
        }
    }

    /// <summary>
    /// Handles updates when the filter bar changes (e.g., user, week, enforce Monday).
    /// Refreshes data based on the new filter values.
    /// </summary>
    /// <param name="filter">Tuple containing filter parameters.</param>
    private async Task OnFilterChanged((int? UserId, DateTime WeekStart, bool EnforceMonday) filter)
    {
        UserId = filter.UserId.Value;
        WeekStart = filter.WeekStart;
        EnforceMonday = filter.EnforceMonday;
        await FetchFilteredData();
    }

    /// <summary>
    /// Opens a blank form for adding a new timesheet entry.
    /// </summary>
    private void OpenAddForm()
    {
        _formModel = new TimesheetEntryFormModel();
        _isFormOpen = true;
    }

    /// <summary>
    /// Expands or collapses a cell in the UI by toggling its ID in the expanded set.
    /// </summary>
    /// <param name="id">The unique identifier of the cell to toggle.</param>
    private void ToggleExpand(Guid id)
    {
        if (!_expandedCells.Add(id))
            _expandedCells.Remove(id);
    }

    /// <summary>
    /// Closes the add/edit form dialog.
    /// </summary>
    private void CloseForm() => _isFormOpen = false;
}
