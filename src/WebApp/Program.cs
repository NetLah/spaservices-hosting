using Azure.Identity;
using Microsoft.AspNetCore.SpaServices;
using NetLah.Diagnostics;
using NetLah.Extensions.ApplicationInsights;
using NetLah.Extensions.Configuration;
using NetLah.Extensions.Logging;
using NetLah.Extensions.SpaServices.Hosting;

AppLog.InitLogger("WebApp");
AppLog.Logger.LogInformation("Application starting...");
try
{
    ApplicationInfo.TryInitialize(null);
    var builder = WebApplication.CreateBuilder(args);
    builder.Configuration
        .AddAddFileConfiguration(options =>
        {
            options.AddProvider("keyPerFile", KeyPerFileConfigurationBuilderExtensions.AddKeyPerFile, resolveAbsolute: true);
            options.AddProvider(".ini", IniConfigurationExtensions.AddIniFile);
            options.AddProvider(".xml", XmlConfigurationExtensions.AddXmlFile);
        })
        .AddTransformConfiguration()
        .AddMapConfiguration();

    builder.UseSerilog();
    var logger = AppLog.Logger;
    void LogAssembly(AssemblyInfo assembly)
    {
        if (assembly.BuildTime.HasValue)
        {
            logger.LogInformation("{title}; Version:{version} BuildTime:{buildTime} Framework:{framework}",
                assembly.Title, assembly.InformationalVersion, assembly.BuildTimestampLocal, assembly.FrameworkName);
        }
        else
        {
            logger.LogInformation("{title}; Version:{version} Framework:{framework}",
                assembly.Title, assembly.InformationalVersion, assembly.FrameworkName);
        }
    }

    var appInfo = builder.InitializeSpaApp();
    logger.LogApplicationLifetimeEvent("Application initializing...", appInfo);

    var appOptions = builder.GetAppOptionsOrDefault();
    if (builder.Environment.IsDevelopment() || appOptions.IsShowAssemblies)
    {
        LogAssembly(new AssemblyInfo(typeof(NetLah.Extensions.Configuration.ConfigurationBuilderBuilder).Assembly));
        LogAssembly(new AssemblyInfo(typeof(NetLah.Extensions.HttpOverrides.HttpOverridesExtensions).Assembly));
        LogAssembly(new AssemblyInfo(typeof(AppLogReference).Assembly));
        LogAssembly(new AssemblyInfo(typeof(AppLog).Assembly));
        LogAssembly(new AssemblyInfo(typeof(AspNetCoreApplicationBuilderExtensions).Assembly));
        LogAssembly(new AssemblyInfo(typeof(WebApplicationBuilderExtensions).Assembly));
        LogAssembly(new AssemblyInfo(typeof(SpaOptions).Assembly));
        LogAssembly(new AssemblyInfo(typeof(Serilog.SerilogApplicationBuilderExtensions).Assembly));
    }

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

public partial class Program { protected Program() { } }
