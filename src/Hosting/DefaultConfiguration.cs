namespace NetLah.Extensions.SpaServices.Hosting;

internal static class DefaultConfiguration
{
    public const string HealthChecksConst = "/_healthz";
    public const string RouteGeneralConst = "_general/{action}";

    public static string HealthChecksPath { get; } = HealthChecksConst;

    public static string RouteGeneral { get; } = RouteGeneralConst;
}
