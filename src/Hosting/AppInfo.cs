using System.Diagnostics;

namespace NetLah.Extensions.SpaServices.Hosting;

internal class AppInfo : IAppInfo
{
    public static AppInfo? Instance { get; set; }
    public static IAppInfo GetInstanceOrDefault() => Instance ?? new AppInfo();

    private readonly Stopwatch _uptime = Stopwatch.StartNew();

    public string InformationalVersion { get; set; } = "";
    public string Title { get; set; } = "";
    public string FrameworkName { get; set; } = "";
    public string BuildTimestampLocal { get; set; } = "";
    public DateTimeOffset StartTime { get; } = DateTimeOffset.Now;
    public TimeSpan Uptime => _uptime.Elapsed;
}
