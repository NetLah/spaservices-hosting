namespace NetLah.Extensions.SpaServices.Hosting;

public class AppOptions
{
    public bool HttpsRedirectionEnabled { get; set; }
    public bool HstsEnabled { get; set; }
    public bool IsShowAssemblies { get; set; }

    public string? WwwRoot { get; set; }
    public string? DefaultPage { get; set; }    // "/index.html"
    public string RouteGeneral { get; set; } = DefaultConfiguration.RouteGeneral;
    public string? RouteGeneralVersion { get; set; }
    public string? RouteGeneralInfo { get; set; }
    public string? RouteGeneralSys { get; set; }

    /// <summary>
    /// /debug/routes
    /// </summary>
    public string? DebugRoutes { get; set; }
}
