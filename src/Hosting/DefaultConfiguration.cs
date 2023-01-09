namespace NetLah.Extensions.SpaServices.Hosting;

internal static class DefaultConfiguration
{
    public const string RouteGeneralConst = "_general/{action}";

    public static string RouteGeneral { get; } = RouteGeneralConst;
}
