using Azure.Core;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetLah.Extensions.Logging;

namespace NetLah.Extensions.ApplicationInsights;

public static class ApplicationInsightsExtensions
{
    private static readonly Lazy<ILogger?> _loggerLazy = new(() => AppLogReference.GetAppLogLogger(typeof(ApplicationInsightsExtensions).Namespace));

    public static WebApplicationBuilder CustomApplicationInsightsTelemetry(this WebApplicationBuilder builder, Func<TokenCredential> tokenCredentialFactory, ILogger? logger = null)
    {
        var isDisabled = builder.Configuration.GetValue<bool>("ApplicationInsights:IsDisabled");
        if (!isDisabled)
        {
            logger ??= _loggerLazy.Value!;
            logger?.LogDebug("ApplicationInsights configured");
            builder.Services.AddApplicationInsightsTelemetry(builder.Configuration);

            var apiKey = builder.Configuration["ApplicationInsights:AuthenticationApiKey"];
            if (!string.IsNullOrEmpty(apiKey))
            {
                logger?.LogDebug("ApplicationInsights Live Metrics with AuthenticationApiKey");
                builder.Services.ConfigureTelemetryModule<QuickPulseTelemetryModule>((module, opt) => module.AuthenticationApiKey = apiKey);
            }
            else
            {
                var isEnabledLiveMetrics = builder.Configuration.GetValue<bool>("ApplicationInsights:IsEnabledLiveMetrics");
                if (isEnabledLiveMetrics)
                {
                    logger?.LogDebug("ApplicationInsights Live Metrics with Azure AD");
                    builder.Services.Configure<TelemetryConfiguration>(config =>
                    {
                        var credential = tokenCredentialFactory();
                        config.SetAzureTokenCredential(credential);
                    });
                }
                else
                {
                    logger?.LogInformation("ApplicationInsights Live Metrics is not configured");
                }
            }
        }

        return builder;
    }
}
