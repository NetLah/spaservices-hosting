using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetLah.Extensions.HttpOverrides;
using NetLah.Extensions.SpaServices.Hosting;

namespace Microsoft.AspNetCore.Builder;

public static class HostBuilderExtentions
{
    public static WebApplicationBuilder AddSpaApp(this WebApplicationBuilder builder, ILogger? logger = null)
    {
        logger ??= LogHelper.LoggerOrDefault;

        var appOptions = builder.Configuration.Get<AppOptions>();
        builder.Services.AddSingleton<AppOptions>(appOptions);

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

        builder.Services.AddSingleton<IAppInfo>(_ => AppInfo.GetInstanceOrDefault());

        builder.Services.AddHealthChecks();

        builder.AddHttpOverrides();

        builder.Services.AddControllers();

        var wwwroot = StringHelper.GetOrDefault(appOptions.WwwRoot, "wwwroot");

        builder.Services.AddSpaStaticFiles(configuration =>
        {
            configuration.RootPath = wwwroot;
        });

        return builder;
    }
}
