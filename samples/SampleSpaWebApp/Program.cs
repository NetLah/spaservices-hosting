using NetLah.Diagnostics;
using NetLah.Extensions.Logging;
using NetLah.Extensions.SpaServices.Hosting;

AppLog.InitLogger("SampleWebApp");
AppLog.Logger.LogInformation("Application starting...");
try
{
    ApplicationInfo.TryInitialize(null);
    var builder = WebApplication.CreateBuilder(args);

    builder.UseSerilog();
    var logger = AppLog.Logger;

    var appInfo = builder.InitializeSpaApp();
    logger.LogApplicationLifetimeEvent("Application initializing...", appInfo);

    IAssemblyInfo assembly = new AssemblyInfo(typeof(ResponseHeadersOptions).Assembly);
    logger.LogInformation("Library:{title}; Version:{version} BuildTime:{buildTime}; Framework:{framework}",
        assembly.Title, assembly.InformationalVersion, assembly.BuildTimestampLocal, assembly.FrameworkName);

    builder.Services.AddApplicationInsightsTelemetry();

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
