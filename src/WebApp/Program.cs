using NetLah.Diagnostics;
using NetLah.Extensions.Logging;

AppLog.InitLogger();
AppLog.Logger.LogInformation("Application starting...");
try
{
    ApplicationInfo.TryInitialize(null);
    var builder = WebApplication.CreateBuilder(args);

    builder.UseSerilog();
    var logger = AppLog.Logger;

    var appInfo = builder.InitializeSpaApp();
    logger.LogApplicationLifetimeEvent("Application initializing... ", appInfo);

    builder.AddSpaApp();

    var app = builder.Build();

    app.UseSpaApp(action: app => app.UseSerilogRequestLoggingLevel());

    app.Logger.LogInformation("Finished configuring application");
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


#pragma warning disable CA1050 // Declare types in namespaces
#pragma warning disable S1118 // Add a 'protected' constructor or the 'static' keyword to the class declaration
public partial class Program { }
#pragma warning restore S1118 // Add a 'protected' constructor or the 'static' keyword to the class declaration
#pragma warning restore CA1050 // Declare types in namespaces
