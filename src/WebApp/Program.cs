using NetLah.Diagnostics;
using NetLah.Extensions.Logging;

AppLog.InitLogger();
AppLog.Logger.LogInformation("Application starting...");
try
{
    _ = ApplicationInfo.TryInitialize(null).CreateAppInfo();

    var builder = WebApplication.CreateBuilder(args);

    builder.UseSerilog(logger => logger.LogApplicationLifetimeEvent("Application initializing... "));
    builder.AddSpaApp();

    var app = builder.Build();

    app.UseSpaApp(action: app => app.UseSerilogRequestLoggingLevel());

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
public partial class Program { }
#pragma warning restore CA1050 // Declare types in namespaces
