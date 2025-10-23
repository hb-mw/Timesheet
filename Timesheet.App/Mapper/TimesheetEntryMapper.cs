using Mapster;
using Timesheet.App.Models.Requests;
using Timesheet.App.Models.Responses;
using Timesheet.Core.Models;

namespace Timesheet.App.Mapper;

public static class TimesheetEntryMapper
{
    public static void Register(TypeAdapterConfig config)
    {
        // requests → domain
        config.NewConfig<UpsertTimesheetEntryRequest, TimesheetEntry>();

        // domain → response
        config.NewConfig<TimesheetEntry, TimesheetEntryResponse>();
        
        config.NewConfig<KeyValuePair<int, decimal>, TotalHoursPerProjectResponse>()
            .Map(dest => dest.ProjectId, src => src.Key)
            .Map(dest => dest.TotalHours, src => src.Value);
    }
}