using Timesheet.API.Middlewares;
using Timesheet.Infra.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddTimesheetInfrastructure();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

var app = builder.Build();


// Configure the HTTP request pipeline.
app.MapOpenApi();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("AllowAll");

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.AddSwaggerUi("Timesheet API");
app.Run();