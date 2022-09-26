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

    public static Task AssertIndexHtmlNotFoundAsync(HttpClient client, string url)
    {
        return AssertIndexHtmlNotFoundAsync(() => client.GetAsync(url));
    }

    public static async Task AssertIndexHtmlNotFoundAsync(Func<Task> handle)
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handle());
        AssertIndexHtmlNotFound(exception);
    }

    private static void AssertIndexHtmlNotFound(InvalidOperationException exception)
    {
        Assert.Equal("The SPA default page middleware could not return the default page '/index.html' because it was not found, and no other middleware handled the request.\n", exception.Message);
    }
}
