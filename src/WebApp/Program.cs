using Azure.Identity;
using NetLah.Diagnostics;
using NetLah.Extensions.ApplicationInsights;
using NetLah.Extensions.Logging;

AppLog.InitLogger("WebApp");
AppLog.Logger.LogInformation("Application starting...");
try
{
    ApplicationInfo.TryInitialize(null);
    var builder = WebApplication.CreateBuilder(args);

    builder.UseSerilog();
    var logger = AppLog.Logger;
    void LogAssembly(AssemblyInfo assembly)
    {
        logger.LogInformation("{title}; Version:{version} Framework:{framework}",
        assembly.Title, assembly.InformationalVersion, assembly.FrameworkName);
    }

    var appInfo = builder.InitializeSpaApp();
    logger.LogApplicationLifetimeEvent("Application initializing...", appInfo);

#if DEBUG
    LogAssembly(new AssemblyInfo(typeof(WebApplicationBuilderExtensions).Assembly));
    LogAssembly(new AssemblyInfo(typeof(Serilog.SerilogApplicationBuilderExtensions).Assembly));
#endif

    builder.CustomApplicationInsightsTelemetry(() => new DefaultAzureCredential());

    builder.AddSpaApp();

    var app = builder.Build();
    logger.LogInformation("Configure the application...");

    app.UseSpaApp(action: app => app.UseSerilogRequestLoggingLevel());

    logger.LogInformation("Finished configuring application");
    app.Run();
}
catch (Exception ex)
{
    AppLog.Logger.LogCritical(ex, "Application terminated unexpectedly");
}
finally
{
    Serilog.Log.CloseAndFlush();
}

public partial class Program { }
