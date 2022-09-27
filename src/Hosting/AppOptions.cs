namespace NetLah.Extensions.SpaServices.Hosting;

public class AppOptions
{
    /// <summary>
    /// /_healthz
    /// </summary>
    public string HealthChecksPath { get; set; } = DefaultConfiguration.HealthChecksPath;

    public bool HttpsRedirectionEnabled { get; set; }
    public bool HstsEnabled { get; set; }

    public string? WwwRoot { get; set; }
    public string RouteGeneral { get; set; } = DefaultConfiguration.RouteGeneral;
    public string? RouteGeneralVersion { get; set; }
    public string? RouteGeneralInfo { get; set; }
    public string? RouteGeneralSys { get; set; }

    /// <summary>
    /// /debug/routes
    /// </summary>
    public string? DebugRoutes { get; set; }
}
