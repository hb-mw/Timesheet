using Mapster;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Timesheet.UI;
using Timesheet.UI.Mapper;
using Timesheet.UI.Services.App;
using Timesheet.UI.Services.Data;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


var baseAddress = new Uri(builder.HostEnvironment.BaseAddress);

Console.WriteLine(builder.HostEnvironment.Environment);

if (builder.HostEnvironment.Environment == "Development")
{
    var localApi = builder.Configuration["ApiUrl"];
    ArgumentNullException.ThrowIfNull(localApi);
    baseAddress = new Uri(localApi);
}


// ✅ optional: read the API relative path from config
var apiPath ="api/Timesheet/";

// ✅ create HttpClient that points to the same origin
builder.Services.AddScoped(sp =>
{
    var uri = new Uri(baseAddress, apiPath);
    return new HttpClient { BaseAddress = uri };
});

// App Services, business logic etc..
builder.Services.AddScoped<TimesheetAppService>();

// Data Services, data access. Does not contain business logic nor care about UI.
builder.Services.AddScoped<TimesheetDataService>();

builder.Services.AddMapster();
var config = TypeAdapterConfig.GlobalSettings;
MapsterConfig.Register(config);

// MudBlazor
builder.Services.AddMudServices();

await builder.Build().RunAsync();