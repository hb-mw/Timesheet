using Mapster;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Timesheet.UI;
using Timesheet.UI.Mapper;
using Timesheet.UI.Services;
using Timesheet.UI.Services.App;
using Timesheet.UI.Services.Data;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiUrl"];
ArgumentNullException.ThrowIfNull(apiBaseUrl);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

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