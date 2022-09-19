using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetLah.Extensions.HttpOverrides;
using NetLah.Extensions.SpaServices.Hosting;

namespace Microsoft.AspNetCore.Builder;

public static class HostingApplicationBuilderExtentions
{
    public static WebApplication UseSpaApp(this WebApplication app, ILogger? logger = null, Action<WebApplication>? action = null)
    {
        logger ??= LogHelper.LoggerOrDefault;

        var appOptions = app.Services.GetRequiredService<AppOptions>();
        var appInfo = app.Services.GetRequiredService<IAppInfo>();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            // todo: #4
            // app.UseHsts();
        }

        var healthChecksPath = StringHelper.GetOrDefault(appOptions.HealthChecksPath, "/my-healthz");
        app.UseHealthChecks(healthChecksPath);
        logger.LogInformation("HealthChecksPath: {healthChecksPath}", healthChecksPath);

        app.UseHttpOverrides();

        if (appOptions.HttpsRedirectionEnabled)
        {
            app.UseHttpsRedirection();
        }
        logger.LogInformation("HttpsRedirectionEnabled: {httpsRedirectionEnabled}", appOptions.HttpsRedirectionEnabled);

        action?.Invoke(app);

        app.UseSpaStaticFiles();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.UseSpa(spa =>
        {
            // do nothing
        });

        app.Lifetime.ApplicationStarted.Register(() => logger.LogApplicationLifetimeEvent("ApplicationStarted", appInfo));
        app.Lifetime.ApplicationStopping.Register(() => logger.LogApplicationLifetimeEvent("ApplicationStopping", appInfo));
        app.Lifetime.ApplicationStopped.Register(() => logger.LogApplicationLifetimeEvent("ApplicationStopped", appInfo));

        return app;
    }
}
