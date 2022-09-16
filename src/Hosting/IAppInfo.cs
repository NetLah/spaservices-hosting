namespace NetLah.Extensions.SpaServices.Hosting;

public interface IAppInfo
{
    string InformationalVersion { get; }
    string Title { get; }
    string FrameworkName { get; }
    string BuildTimestampLocal { get; }
    DateTimeOffset StartTime { get; }
    TimeSpan Uptime { get; }
}
