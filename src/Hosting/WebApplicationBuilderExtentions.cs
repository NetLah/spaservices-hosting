﻿using Microsoft.Extensions.Configuration;
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

        var appOptions = builder.Configuration.Get<AppOptions>();
        builder.SetAppOptions(appOptions);

        var wwwroot = StringHelper.GetOrDefault(appOptions.WwwRoot, "wwwroot");
        logger.LogDebug("Load UI version information {folder}", wwwroot);
        var appFileVersionInfo = new AppFileVersionParser().ParseFolder(wwwroot);

        var appInfo = builder.GetAppInfoOrDefault().BindAppInfo(ApplicationInfo.Instance, appFileVersionInfo ?? new AppFileVersionInfo());

        builder.Services.AddSingleton<AppOptions>(appOptions);
        builder.Services.AddSingleton<IAppInfo>(appInfo);

        return appInfo;
    }

    public static WebApplicationBuilder AddSpaApp(this WebApplicationBuilder builder, ILogger? logger = null)
    {
        logger ??= LogHelper.LoggerOrDefault;

        var appOptions = builder.GetAppOptionsOrDefault();

        // todo: #4
        //var hstsConfiguration = builder.Configuration.GetSection("Hsts");
        //builder.Services.AddHsts(options =>
        //{
        //    if (!string.IsNullOrEmpty(hstsConfiguration["Preload"]) ||
        //    !string.IsNullOrEmpty(hstsConfiguration["IncludeSubDomains"]) ||
        //    !string.IsNullOrEmpty(hstsConfiguration["MaxAge"]) ||
        //    !string.IsNullOrEmpty(hstsConfiguration["ExcludedHosts"]))
        //    {
        //        hstsConfiguration.Bind(options);
        //    }
        //    else
        //    {
        //        options.Preload = true;
        //        options.IncludeSubDomains = true;
        //        options.MaxAge = TimeSpan.FromDays(365);
        //    }
        //});

        builder.Services.AddHealthChecks();

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

    internal static AppInfo GetAppInfoOrDefault(this WebApplicationBuilder builder)
    {
        if (!builder.Host.Properties.TryGetValue(typeof(AppInfo), out var appInfoObject) || appInfoObject is not AppInfo appInfo)
        {
            appInfo = new AppInfo();
            SetAppInfo(builder, appInfo);
        }
        return appInfo;
    }

    internal static WebApplicationBuilder SetAppInfo(this WebApplicationBuilder builder, AppInfo appInfo)
    {
        builder.Host.Properties[typeof(AppInfo)] = appInfo ?? throw new ArgumentNullException(nameof(appInfo));
        return builder;
    }

    internal static AppOptions GetAppOptionsOrDefault(this WebApplicationBuilder builder)
    {
        if (!builder.Host.Properties.TryGetValue(typeof(AppOptions), out var appOptionsObject) || appOptionsObject is not AppOptions appOptions)
        {
            appOptions = new AppOptions();
            SetAppOptions(builder, appOptions);
        }
        return appOptions;
    }

    internal static WebApplicationBuilder SetAppOptions(this WebApplicationBuilder builder, AppOptions appOptions)
    {
        builder.Host.Properties[typeof(AppOptions)] = appOptions ?? throw new ArgumentNullException(nameof(appOptions));
        return builder;
    }
}