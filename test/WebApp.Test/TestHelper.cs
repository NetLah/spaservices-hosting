using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;

namespace WebApp.Test;

internal static class TestHelper
{
    public static readonly string ContentRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../.."));

    public static WebApplicationBuilder CreateWebApplicationBuilder(string environmentName = DefaultConfig.TestingEnvironmentConst)
    {
        var builder = WebApplication.CreateBuilder(
            new[] {
                $"--environment={environmentName}",
                $"--contentRoot={ContentRoot}",
                "--applicationNAME=WebApp",
                }
            );

        builder.WebHost.UseTestServer();

        return builder;
    }

    public static async Task AssertIndexHtmlNotFound(HttpClient client, string url)
    {
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => client.GetAsync(url));
        Assert.Equal("The SPA default page middleware could not return the default page '/index.html' because it was not found, and no other middleware handled the request.\n", ex.Message);
    }
}
