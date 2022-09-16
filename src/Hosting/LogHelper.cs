using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NetLah.Extensions.Logging;

namespace NetLah.Extensions.SpaServices.Hosting;

internal static class LogHelper
{
    public static readonly Lazy<ILogger?> LoggerLazy = new(() => AppLogReference.GetAppLogLogger("NetLah.Extensions.SpaServices"));

    public static ILogger? Logger => LoggerLazy.Value;

    public static ILogger LoggerOrDefault => Logger ?? NullLogger.Instance;
}
