using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Timesheet.Infra.Extensions;

public static class Swagger
{
    public static IApplicationBuilder AddSwaggerUi(this IApplicationBuilder app, string name)
    {
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/openapi/v1.json", name);
        });
        
        var lifetime = app.ApplicationServices.GetRequiredService<Microsoft.Extensions.Hosting.IHostApplicationLifetime>();
        lifetime.ApplicationStarted.Register(() =>
        {
            var server = app.ApplicationServices.GetRequiredService<Microsoft.AspNetCore.Hosting.Server.IServer>();
            var addressesFeature = server.Features.Get<IServerAddressesFeature>();

            if (addressesFeature == null) return;
            foreach (var address in addressesFeature.Addresses)
            {
                Console.WriteLine($"Swagger UI available at: {address}/swagger/index.html");
            }
        });

        return app;
    }
}