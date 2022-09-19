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

    public string Title { get; set; } = "";
    public string Version { get; set; } = "";
    public string? BuildTimestampLocal { get; set; }
    public string? Description { get; set; }

    public string AssemblyTitle { get; set; } = "";
    public string AssemblyInformationalVersion { get; set; } = "";
    public string AssemblyBuildTimestampLocal { get; set; } = "";
    public string AssemblyFrameworkName { get; set; } = "";
}
