using NetLah.Diagnostics;
using System.Diagnostics;

namespace NetLah.Extensions.SpaServices.Hosting;

internal class AppInfo : IAppInfo
{
    private readonly Stopwatch _uptime = Stopwatch.StartNew();

    public IAssemblyInfo? AssemblyInfo { get; set; }
    public AppFileVersionInfo? AppFileVersionInfo { get; set; }

    public DateTimeOffset StartTime { get; } = DateTimeOffset.Now;
    public TimeSpan Uptime => _uptime.Elapsed;

    public string Title { get; set; } = default!;
    public string Version { get; set; } = default!;
    public string? BuildTimestampLocal { get; set; }
    public string? Description { get; set; }

    public string HostTitle { get; set; } = default!;
    public string HostInformationalVersion { get; set; } = default!;
    public string HostBuildTimestampLocal { get; set; } = default!;
    public string HostFrameworkName { get; set; } = default!;
}
