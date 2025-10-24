using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;

namespace Timesheet.UI.Components;

public partial class TimesheetFilterBar : ComponentBase
{
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Parameter] public int? UserId { get; set; }

    [Parameter] public DateTime WeekStart { get; set; }

    [Parameter] public bool EnforceMonday { get; set; }

    [Parameter]
    public EventCallback<(int? UserId, DateTime WeekStart, bool EnforceMonday)> OnFilterChanged { get; set; }

    [Parameter] public EventCallback OnAddClicked { get; set; }

    [Parameter] public bool ShowAddButton { get; set; } = true;

    private void OnEnforceMondayChanged(bool value)
    {
        EnforceMonday = value;
    }

    private async Task OnWeekChanged(DateTime? newDate)
    {
        if (newDate is null) return;

        var adjusted = EnforceMonday
            ? newDate.Value.StartOfWeek(DayOfWeek.Monday)
            : newDate.Value;

        WeekStart = adjusted;
        await OnFilterChanged.InvokeAsync((UserId, WeekStart, EnforceMonday));
    }

    private async Task OnUserIdChanged(int? newId)
    {
        if (newId is null || newId.Value == UserId) return;

        if (newId < 1)
        {
            Snackbar.Add("User id must be greater than 0", Severity.Error);
            return;
        }

        UserId = newId;
        await OnFilterChanged.InvokeAsync((UserId, WeekStart, EnforceMonday));
    }

    private void PreviousWeek()
    {
        Console.WriteLine("PREV WEEK");
        WeekStart = WeekStart.AddDays(-7);
    }

    private void NextWeek()
    {
        Console.WriteLine("NEXT WEEK");
        WeekStart = WeekStart.AddDays(7);
    }
}