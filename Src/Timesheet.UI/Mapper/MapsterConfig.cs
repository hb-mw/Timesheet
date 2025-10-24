using Mapster;

namespace Timesheet.UI.Mapper;

public class MapsterConfig
{
    public static void Register(TypeAdapterConfig config)
    {
        TimesheetEntryMapper.Register(config);
    }
}