using Mapster;
using Timesheet.Shared.Contracts;
using Timesheet.UI.Models.Forms;

namespace Timesheet.UI.Mapper;

public static class TimesheetEntryMapper
{
    public static void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TimesheetEntryFormModel, UpsertTimesheetEntryRequest>()
            .Map(dest => dest.Date,
                src => DateOnly.FromDateTime(src.Date!.Value));
    }
}