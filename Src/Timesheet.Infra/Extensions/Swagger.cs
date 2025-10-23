using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Timesheet.Infra.Extensions;

public static class Swagger
{
    public static IApplicationBuilder AddSwaggerUi(this IApplicationBuilder app, string name)
    {
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/openapi/v1.json", name); });

        var lifetime =
            app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStarted.Register(() =>
        {
            var server = app.ApplicationServices.GetRequiredService<IServer>();
            var addressesFeature = server.Features.Get<IServerAddressesFeature>();

            if (addressesFeature == null) return;
            foreach (var address in addressesFeature.Addresses)
                Console.WriteLine($"Swagger UI available at: {address}/swagger/index.html");
        });

        return app;
    }
}