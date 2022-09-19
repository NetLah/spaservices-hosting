using NetLah.Extensions.SpaServices.Hosting;

namespace NetLah.Diagnostics;

internal static class AppInfoExtensions
{
    public static AppInfo BindAppInfo(this AppInfo appInfo, IAssemblyInfo assemblyInfo, AppFileVersionInfo appFileVersionInfo)
    {
        if (appInfo == null) { throw new ArgumentNullException(nameof(appInfo)); }

        appInfo.AppFileVersionInfo = appFileVersionInfo ?? throw new ArgumentNullException(nameof(appFileVersionInfo));
        appInfo.AssemblyInfo = assemblyInfo ?? throw new ArgumentNullException(nameof(assemblyInfo));

        appInfo.Title = appFileVersionInfo.Title ?? assemblyInfo.Title;
        appInfo.Version = appFileVersionInfo.Version ?? assemblyInfo.InformationalVersion;
        appInfo.BuildTimestampLocal = GetTimestampString(appFileVersionInfo.BuildTime, TimeZoneInfo.Local);
        appInfo.Description = appFileVersionInfo.Description;

        appInfo.AssemblyTitle = assemblyInfo.Title;
        appInfo.AssemblyInformationalVersion = assemblyInfo.InformationalVersion;
        appInfo.AssemblyBuildTimestampLocal = assemblyInfo.BuildTimestampLocal;
        appInfo.AssemblyFrameworkName = assemblyInfo.FrameworkName;

        return appInfo;
    }

    private static string? GetTimestampString(DateTimeOffset? dateTimeOffset, TimeZoneInfo timeZoneInfo)
    {
        if (!dateTimeOffset.HasValue)
        {
            return default;
        }

        var local = TimeZoneInfo.ConvertTime(dateTimeOffset.Value, timeZoneInfo);
        var localString = local.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz", System.Globalization.CultureInfo.InvariantCulture);
        return localString;
    }
}
