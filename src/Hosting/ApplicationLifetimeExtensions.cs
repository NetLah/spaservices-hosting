using Microsoft.Extensions.Logging;
using NetLah.Extensions.SpaServices.Hosting;

namespace Microsoft.AspNetCore.Builder;

public static class ApplicationLifetimeExtensions
{
    public static void LogApplicationLifetimeEvent(this ILogger logger, string lifetime, IAppInfo appInfo)
    {
        logger.LogInformation("{lifetime}\r\n" +
            "AppTitle:{appTitle}; Version:{version}; BuildTime:{buildTime}\r\n" +
            "Host:{hostApp}; Version:{hostVersion}; BuildTime:{hostBuildTime}; Framework:{framework}\r\n" +
            "Description:{description}",
            lifetime, appInfo.Title, appInfo.Version, appInfo.BuildTimestampLocal,
            appInfo.HostTitle, appInfo.HostInformationalVersion, appInfo.HostBuildTimestampLocal, appInfo.HostFrameworkName,
            appInfo.Description);
    }
}
