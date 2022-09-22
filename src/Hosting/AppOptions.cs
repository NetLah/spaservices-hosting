namespace NetLah.Extensions.SpaServices.Hosting;

public class AppOptions
{
    /// <summary>
    /// /_healthz
    /// </summary>
    public string HealthChecksPath { get; set; } = DefaultConfiguration.HealthChecksPath;

    public bool HttpsRedirectionEnabled { get; set; } = true;
    public bool HstsEnabled { get; set; } = true;

    public string? WwwRoot { get; set; }
    public string RouteGeneral { get; set; } = "api/v1/{controller}/{action}";
    public string? RouteGeneralGetInfo { get; set; }
    public string? RouteGeneralSysInfo { get; set; }

    /// <summary>
    /// /debug/routes
    /// </summary>
    public string? DebugRoutes { get; set; }
}
