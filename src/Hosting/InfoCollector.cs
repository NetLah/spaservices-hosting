using Microsoft.AspNetCore.Hosting;
using NetLah.Common;

namespace NetLah.Extensions.SpaServices.Hosting;

public class InfoCollector : IInfoCollector
{
    public List<string> Logs { get; } = new List<string>();

    public IInfoCollector Add<T>(string key, T value)
    {
        Logs.Add($"{key}: {value}");
        return this;
    }

    public static IInfoCollector CreateInstance(IWebHostEnvironment env, IAppInfo appInfo)
        => new InfoCollector()
            .Add("App", appInfo.Title)
            .Add("Version", appInfo.Version)
            .Add("BuildTime", appInfo.BuildTimestampLocal)
            .Add("Description", appInfo.Description)
            .Add("Host", appInfo.HostTitle)
            .Add("HostVersion", appInfo.HostInformationalVersion)
            .Add("HostFramework", appInfo.HostFrameworkName)
            .Add("HostBuildTime", appInfo.HostBuildTimestampLocal)
            .Add("Environment", env.EnvironmentName)
            .Add("Timezone", TimeZoneInfo.Local.DaylightName)
            .Add("TimezoneSG", TimeZoneLocalHelper.GetSingaporeOrCustomTimeZone())
            .Add("ContentRootPath", env.ContentRootPath)
            .Add("RootPath (PWD)", Directory.GetCurrentDirectory())
            .Add("BaseDirectory", AppDomain.CurrentDomain.BaseDirectory)
            .Add("StartTime", appInfo.StartTime);
}
