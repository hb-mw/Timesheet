using Mapster;
using Timesheet.App.Mapper;

namespace Timesheet.Tests.Controllers.Fixtures;

[CollectionDefinition("MapsterConfig")]
public sealed class MapsterConfigCollection : ICollectionFixture<MapsterBootstrap>
{
}

public class MapsterBootstrap
{
    private static bool _initialized;

    public MapsterBootstrap()
    {
        if (_initialized) return;

        // Register Mapper Config
        TimesheetEntryMapper.Register(TypeAdapterConfig.GlobalSettings);

        _initialized = true;
    }
}