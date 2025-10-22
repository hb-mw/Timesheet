using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Timesheet.App.Services;
using Timesheet.Core.Interfaces.Repositories;
using Timesheet.Core.Interfaces.Services;
using Timesheet.Infra.Data.Repositories;

namespace Timesheet.Infra.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTimesheetInfrastructure(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "Timesheet API",
                    Version = "v1",
                    Description = "API for managing timesheet entries. made with ❤️ for Cmap Team to enjoy reviewing :)",
                    Contact = new OpenApiContact
                    {
                        Name = "Mahmoud(Martin) Wizzo",
                        Email = "mahmoud.wizzo@gmail.com",
                        Url = new Uri("https://github.com/hb-mw/Timesheet")
                    }
                };
                return Task.CompletedTask;
            });
        });
        
        // Repositories
        services.AddSingleton<ITimesheetEntryRepository, InMemoryTimesheetEntryRepository>();
        
        //Services
        services.AddScoped<ITimesheetEntryService, TimesheetEntryService>();
        
        return services;
    }

    
}