namespace NetLah.Extensions.SpaServices.Hosting;

public interface IAppInfo
{
    DateTimeOffset StartTime { get; }
    TimeSpan Uptime { get; }

    string Title { get; }
    string Version { get; }
    string? BuildTimestampLocal { get; }
    string? Description { get; }

    string HostTitle { get; }
    string HostInformationalVersion { get; }
    string HostBuildTimestampLocal { get; }
    string HostFrameworkName { get; }
}
