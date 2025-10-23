using FluentValidation;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using Timesheet.App.Mapper;
using Timesheet.App.Services;
using Timesheet.App.Validators;
using Timesheet.Core.Interfaces.Repositories;
using Timesheet.Core.Interfaces.Services;
using Timesheet.Infra.Data.Repositories;

namespace Timesheet.Infra.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTimesheetInfrastructure(this IServiceCollection services)
    {
        // Repositories
        services.AddSingleton<ITimesheetEntryRepository, InMemoryTimesheetEntryRepository>();

        // Services
        services.AddScoped<ITimesheetEntryService, TimesheetEntryService>();

        // Mapper - Mapster
        services.AddMapster();
        var config = TypeAdapterConfig.GlobalSettings;
        MapsterConfig.Register(config);

        // Validators
        services.AddValidatorsFromAssemblyContaining<UpsertTimesheetEntryRequestValidator>();

        // Fail fast
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

        // AutoValidation
        services.AddFluentValidationAutoValidation();

        // OpenAPI
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "Timesheet API",
                    Version = "v1",
                    Description =
                        "API for managing timesheet entries. made with ❤️ for Cmap Team to enjoy reviewing :)",
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

        return services;
    }
}