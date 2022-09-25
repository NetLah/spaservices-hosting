using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetLah.Extensions.HttpOverrides;
using NetLah.Extensions.SpaServices.Hosting;
using NetLah.Extensions.SpaServices.Hosting.Controllers;
using System.Text;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtentions
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
            if (appOptions.HstsEnabled)
            {
                app.UseHsts();
            }
        }
        logger.LogInformation("Use Hsts: {enalbed}", !app.Environment.IsDevelopment() && appOptions.HstsEnabled);

        var healthChecksPath = StringHelper.GetOrDefault(appOptions.HealthChecksPath, DefaultConfiguration.HealthChecksPath);
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

        app.TryAddGeneralInfo(appOptions, logger);

        if (appOptions.DebugRoutes is string debugRoutes && !string.IsNullOrWhiteSpace(debugRoutes))
        {
            logger.LogDebug("Debug routes: {debugRoutes}", debugRoutes);

            app.MapGet(debugRoutes, (IEnumerable<EndpointDataSource> endpointSources) =>
            {
                var sb = new StringBuilder();
                var endpoints = endpointSources.SelectMany(es => es.Endpoints);
                foreach (var endpoint in endpoints)
                {
                    var routeNameMetadata = endpoint.Metadata.OfType<RouteNameMetadata>().FirstOrDefault();
                    var httpMethodsMetadata = endpoint.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault();

                    sb.Append($"[{routeNameMetadata?.RouteName}] {(httpMethodsMetadata == null ? null : string.Join(",", httpMethodsMetadata.HttpMethods))}");
                    if (endpoint is RouteEndpoint routeEndpoint)
                    {
                        sb.AppendLine($" {routeEndpoint.RoutePattern.RawText} {routeEndpoint.DisplayName}");
                    }
                }
                return sb.ToString();
            });
        }

        app.Lifetime.ApplicationStarted.Register(() => logger.LogApplicationLifetimeEvent("Application started", appInfo));
        app.Lifetime.ApplicationStopping.Register(() => logger.LogApplicationLifetimeEvent("Application stopping", appInfo));
        app.Lifetime.ApplicationStopped.Register(() => logger.LogApplicationLifetimeEvent("Application stopped", appInfo));

        return app;
    }

    private static WebApplication TryAddGeneralInfo(this WebApplication app, AppOptions appOptions, ILogger logger)
    {
        var routeGeneralVersion = StringHelper.NormalizeNull(appOptions.RouteGeneralVersion);
        var routeGeneralInfo = StringHelper.NormalizeNull(appOptions.RouteGeneralInfo);
        var routeGeneralSys = StringHelper.NormalizeNull(appOptions.RouteGeneralSys);
        var routeGeneral = StringHelper.NormalizeNull(appOptions.RouteGeneral);

        if (routeGeneralVersion != null)
        {
            logger.LogDebug("Map General/Version {route}", routeGeneralInfo);
            app.MapControllerRoute(name: "General/Version",
                pattern: routeGeneralVersion,
                defaults: new { controller = "General", action = nameof(GeneralController.Version) })
                .WithMetadata(new HttpMethodMetadata(new[] { "GET" }));
        }

        if (routeGeneralInfo != null)
        {
            logger.LogDebug("Map General/Info {route}", routeGeneralInfo);
            app.MapControllerRoute(name: "General/Info",
                pattern: routeGeneralInfo,
                defaults: new { controller = "General", action = nameof(GeneralController.Info) })
                .WithMetadata(new HttpMethodMetadata(new[] { "GET" }));
        }

        if (routeGeneralSys != null)
        {
            logger.LogDebug("Map General/Sys {route}", routeGeneralSys);
            app.MapControllerRoute(name: "General/Sys",
                pattern: routeGeneralSys,
                defaults: new { controller = "General", action = nameof(GeneralController.Sys) })
                .WithMetadata(new HttpMethodMetadata(new[] { "GET" }));
        }

        if (routeGeneralInfo == null && routeGeneralSys == null && routeGeneral != null)
        {
            logger.LogDebug("Map GeneralController {route}", routeGeneral);
            app.MapControllerRoute(name: "GeneralController",
                pattern: routeGeneral,
                defaults: new { controller = "General" })
                .WithMetadata(new HttpMethodMetadata(new[] { "GET" }));
        }

        return app;
    }
}
