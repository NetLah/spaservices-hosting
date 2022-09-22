namespace NetLah.Extensions.SpaServices.Hosting;

internal static class DefaultConfiguration
{
    public const string HealthChecks = "/_healthz";

    public static string HealthChecksPath { get; } = HealthChecks;
}
