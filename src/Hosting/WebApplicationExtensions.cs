using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetLah.Extensions.HttpOverrides;
using NetLah.Extensions.SpaServices.Hosting;
using NetLah.Extensions.SpaServices.Hosting.Controllers;
using System.Text;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtensions
{
    private static readonly string[] DefaultHttpMethods = new[] { "GET" };

    public static WebApplication UseSpaApp(this WebApplication app, ILogger? logger = null, Action<WebApplication>? action = null)
    {
        logger ??= LogHelper.LoggerOrDefault;

        var appOptions = app.Services.GetRequiredService<AppOptions>();
        var appInfo = app.Services.GetRequiredService<IAppInfo>();

        app.UseHttpOverrides(); // v0.2.3 support HealthChecks configurable

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
        logger.LogInformation("Use Hsts: {enabled}", !app.Environment.IsDevelopment() && appOptions.HstsEnabled);

        if (appOptions.HttpsRedirectionEnabled)
        {
            app.UseHttpsRedirection();
        }
        logger.LogInformation("HttpsRedirectionEnabled: {httpsRedirectionEnabled}", appOptions.HttpsRedirectionEnabled);

        action?.Invoke(app);

        var staticFileOptions = new StaticFileOptions();

        var responseHeaders = appOptions.ResponseHeaders;

        if (responseHeaders.List is { } list && list.Count != 0)
        {
            foreach (var item in list)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    var pos = item.IndexOf('=');
                    if (pos > 0)
                    {
                        var hKey = item[..pos];
                        var hValue = item[(pos + 1)..];
                        AddHeader(hKey, hValue);
                    }
                }
            }

            void AddHeader(string key, string value)
            {
                responseHeaders.Headers ??= new Dictionary<string, string?>();
                responseHeaders.Headers.TryAdd(key, value);
            }
        }

        var isResponseHeadersEnabled = responseHeaders != null
            && responseHeaders.Headers is { } headers
            && headers.Count != 0
            && responseHeaders.IsEnabled;
        if (isResponseHeadersEnabled && responseHeaders != null)
        {
            var headerNames = responseHeaders.Headers.Keys.OrderBy(i => i, StringComparer.OrdinalIgnoreCase).ToArray();
            logger.LogInformation("ResponseHeadersHandler Setup for {headerNames}", (object)headerNames);
            var handler = new ResponseHeadersHandler(logger, responseHeaders);
            staticFileOptions.OnPrepareResponse = handler.PrepareResponse;
        }

        app.UseSpaStaticFiles(staticFileOptions);

        app.UseRouting();

        app.UseGeneralInfo(appOptions, logger);

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

        app.UseSpa(spa =>
        {
            spa.Options.DefaultPageStaticFileOptions = staticFileOptions;
            if (appOptions.DefaultPage is { } defaultPage)
            {
                spa.Options.DefaultPage = defaultPage;
                logger.LogInformation("SPA DefaultPage: {defaultPage}", defaultPage);
            }
        });

        app.Lifetime.ApplicationStarted.Register(() => logger.LogApplicationLifetimeEvent("Application started", appInfo));
        app.Lifetime.ApplicationStopping.Register(() => logger.LogApplicationLifetimeEvent("Application stopping", appInfo));
        app.Lifetime.ApplicationStopped.Register(() => logger.LogApplicationLifetimeEvent("Application stopped", appInfo));

        return app;
    }

    private static WebApplication UseGeneralInfo(this WebApplication app, AppOptions appOptions, ILogger logger)
    {
        var mapControllerAction = true;
        var routeGeneralVersion = StringHelper.NormalizeNull(appOptions.RouteGeneralVersion);
        var routeGeneralInfo = StringHelper.NormalizeNull(appOptions.RouteGeneralInfo);
        var routeGeneralSys = StringHelper.NormalizeNull(appOptions.RouteGeneralSys);

        if (routeGeneralVersion == null && routeGeneralInfo == null && routeGeneralSys == null
            && StringHelper.NormalizeNull(appOptions.RouteGeneral) is { } routeGeneral)
        {
            mapControllerAction = false;
            logger.LogDebug("Map General {route}", routeGeneral);
            routeGeneralVersion = routeGeneral.Replace("{action}", nameof(GeneralController.Version), StringComparison.OrdinalIgnoreCase);
            routeGeneralInfo = routeGeneral.Replace("{action}", nameof(GeneralController.Info), StringComparison.OrdinalIgnoreCase);
            routeGeneralSys = routeGeneral.Replace("{action}", nameof(GeneralController.Sys), StringComparison.OrdinalIgnoreCase);
        }

        if (routeGeneralVersion != null)
        {
            if (mapControllerAction)
            {
                logger.LogDebug("Map General/Version {route}", routeGeneralInfo);
            }

            app.MapControllerRoute(name: string.Empty,
                pattern: routeGeneralVersion,
                defaults: new { controller = "General", action = nameof(GeneralController.Version) })
                .WithMetadata(new HttpMethodMetadata(DefaultHttpMethods));
        }

        if (routeGeneralInfo != null)
        {
            if (mapControllerAction)
            {
                logger.LogDebug("Map General/Info {route}", routeGeneralInfo);
            }

            app.MapControllerRoute(name: string.Empty,
                pattern: routeGeneralInfo,
                defaults: new { controller = "General", action = nameof(GeneralController.Info) })
                .WithMetadata(new HttpMethodMetadata(DefaultHttpMethods));
        }

        if (routeGeneralSys != null)
        {
            if (mapControllerAction)
            {
                logger.LogDebug("Map General/Sys {route}", routeGeneralSys);
            }

            app.MapControllerRoute(name: string.Empty,
                pattern: routeGeneralSys,
                defaults: new { controller = "General", action = nameof(GeneralController.Sys) })
                .WithMetadata(new HttpMethodMetadata(DefaultHttpMethods));
        }

        return app;
    }
}
