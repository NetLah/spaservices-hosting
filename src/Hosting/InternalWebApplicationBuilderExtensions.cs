using Microsoft.AspNetCore.Builder;

namespace NetLah.Extensions.SpaServices.Hosting;

internal static class InternalWebApplicationBuilderExtensions
{
    public static AppInfo GetAppInfoOrDefault(this WebApplicationBuilder builder)
    {
        if (!builder.Host.Properties.TryGetValue(typeof(AppInfo), out var appInfoObject) || appInfoObject is not AppInfo appInfo)
        {
            appInfo = new AppInfo();
            SetAppInfo(builder, appInfo);
        }
        return appInfo;
    }

    public static WebApplicationBuilder SetAppInfo(this WebApplicationBuilder builder, AppInfo appInfo)
    {
        builder.Host.Properties[typeof(AppInfo)] = appInfo ?? throw new ArgumentNullException(nameof(appInfo));
        return builder;
    }

    public static AppOptions GetAppOptionsOrDefault(this WebApplicationBuilder builder)
    {
        if (!builder.Host.Properties.TryGetValue(typeof(AppOptions), out var appOptionsObject) || appOptionsObject is not AppOptions appOptions)
        {
            appOptions = new AppOptions();
            SetAppOptions(builder, appOptions);
        }
        return appOptions;
    }

    public static WebApplicationBuilder SetAppOptions(this WebApplicationBuilder builder, AppOptions appOptions)
    {
        builder.Host.Properties[typeof(AppOptions)] = appOptions ?? throw new ArgumentNullException(nameof(appOptions));
        return builder;
    }
}
