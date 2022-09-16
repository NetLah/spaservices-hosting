using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace WebApp.Test;

internal static class WebApplicationExtensions
{
    public static HttpClient CreateClientNoAutoRedirect<TEntryPoint>(this WebApplicationFactory<TEntryPoint> factory) where TEntryPoint : class
        => factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    public static IWebHostBuilder RelocateContentRoot(this IWebHostBuilder builder)
    {
        builder.UseSetting("contentRoot", TestHelper.ContentRoot);
        return builder;
    }

    public static IWebHostBuilder SetupTestingEnvironment(this IWebHostBuilder builder, string environmentName = DefaultConfig.TestingEnvironmentConst)
    {
        builder.UseEnvironment(environmentName);
        builder.RelocateContentRoot();
        return builder;
    }
}
