using NetLah.Extensions.SpaServices.Hosting;

namespace NetLah.Diagnostics;

public static class AppInfoExtensions
{
    public static IAppInfo CreateAppInfo(this IAssemblyInfo assemblyInfo) => AppInfo.Instance ??= new()
    {
        BuildTimestampLocal = assemblyInfo.BuildTimestampLocal,
        FrameworkName = assemblyInfo.FrameworkName,
        InformationalVersion = assemblyInfo.InformationalVersion,
        Title = assemblyInfo.Title,
    };
}
