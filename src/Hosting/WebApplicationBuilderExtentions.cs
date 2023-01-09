using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetLah.Diagnostics;
using NetLah.Extensions.HttpOverrides;
using NetLah.Extensions.SpaServices.Hosting;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationBuilderExtentions
{
    public static IAppInfo InitializeSpaApp(this WebApplicationBuilder builder, ILogger? logger = null)
    {
        logger ??= LogHelper.LoggerOrDefault;

        var appOptions = builder.Configuration.Get<AppOptions>()!;
        builder.SetAppOptions(appOptions);

        var wwwroot = StringHelper.GetOrDefault(appOptions.WwwRoot, "wwwroot");
        logger.LogDebug("Load UI version information {folder}", wwwroot);
        var appFileVersionInfo = new AppFileVersionParser().ParseFolder(wwwroot);

        var appInfo = builder.GetAppInfoOrDefault().BindAppInfo(ApplicationInfo.Instance, appFileVersionInfo ?? new AppFileVersionInfo());

        builder.Services.AddSingleton<AppOptions>(appOptions);
        builder.Services.AddSingleton<IAppInfo>(appInfo);
        builder.Services.AddSingleton(builder.SetProperty(InfoCollector.CreateInstance(builder.Environment, appInfo)));

        return appInfo;
    }

    public static WebApplicationBuilder AddSpaApp(this WebApplicationBuilder builder, ILogger? logger = null)
    {
        logger ??= LogHelper.LoggerOrDefault;

        var appOptions = builder.GetAppOptionsOrDefault();

        var hstsConfiguration = builder.Configuration.GetSection("Hsts");
        if (appOptions.HstsEnabled)
        {
            var loading = hstsConfiguration.Exists();
            if (loading)
            {

                builder.Services.AddHsts(options =>
                {
                    if (!string.IsNullOrEmpty(hstsConfiguration["Preload"]) ||
                    !string.IsNullOrEmpty(hstsConfiguration["IncludeSubDomains"]) ||
                    !string.IsNullOrEmpty(hstsConfiguration["MaxAge"]) ||
                    !string.IsNullOrEmpty(hstsConfiguration["ExcludedHosts"]))
                    {
                        hstsConfiguration.Bind(options);
                    }
                });
            }
            logger.LogDebug("Load Hsts configuration {loading}", loading);
        }

        builder.AddHttpOverrides();

        builder.Services.AddControllers();

        var wwwroot = StringHelper.GetOrDefault(appOptions.WwwRoot, "wwwroot");

        logger.LogInformation("Spa static files location {folder}", wwwroot);
        builder.Services.AddSpaStaticFiles(configuration =>
        {
            configuration.RootPath = wwwroot;
        });

        return builder;
    }
}
