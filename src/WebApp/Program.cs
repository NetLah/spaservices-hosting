using NetLah.Diagnostics;
using NetLah.Extensions.Logging;
using NetLah.Extensions.SpaServices.Hosting;

AppLog.InitLogger();
AppLog.Logger.LogInformation("Application starting...");
try
{
    IAppInfo appInfo;
#pragma warning disable S2551 // Shared resources should not be used for locking
    lock (typeof(ApplicationInfo))
    {
        // Fix concurrent testing: System.InvalidOperationException : The entry point exited without ever building an IHost
        appInfo = ApplicationInfo.TryInitialize(null).CreateAppInfo();
    }
#pragma warning restore S2551 // Shared resources should not be used for locking

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
