using Microsoft.Extensions.Logging;
using NetLah.Diagnostics;

namespace Microsoft.AspNetCore.Builder;

public static class ApplicationLifetimeExtensions
{
    public static void LogApplicationLifetimeEvent(this ILogger logger, string lifetime)
    {
        var appInfo = ApplicationInfo.Instance;
        logger.LogInformation("{lifetime}: AppTitle:{appTitle}; Version:{version}; BuildTime:{buildTime}; Framework:{framework}", lifetime, appInfo?.Title, appInfo?.InformationalVersion, appInfo?.BuildTimestampLocal, appInfo?.FrameworkName);
    }
}
