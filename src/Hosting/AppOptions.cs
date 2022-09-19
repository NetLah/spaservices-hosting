namespace NetLah.Extensions.SpaServices.Hosting;

public class AppOptions
{
    public string HealthChecksPath { get; set; } = "/my-healthz";

    public bool HttpsRedirectionEnabled { get; set; } = true;

    public string? WwwRoot { get; set; }
    public string RouteGeneral { get; set; } = "api/v1/{controller}/{action}";
    public string? RouteGeneralGetInfo { get; set; }
    public string? RouteGeneralSysInfo { get; set; }

    /// <summary>
    /// /debug/routes
    /// </summary>
    public string? DebugRoutes { get; set; }
}
