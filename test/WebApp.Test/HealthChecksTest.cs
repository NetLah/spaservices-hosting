using Microsoft.AspNetCore.Mvc.Testing;

namespace WebApp.Test;

public class HealthChecksTest
{
    [Theory]
    [InlineData(null, "healthz", "_healthz", "__gtg")]
    [InlineData("", "_healthz")]    // listen on all URLs
    [InlineData("", "healthz")]     // listen on all URLs
    [InlineData("", "healthz/any")] // listen on all URLs
    [InlineData("", "index.html")]  // listen on all URLs
    [InlineData("/_healthz", "_healthz", "healthz")]
    [InlineData("/__gtg", "__gtg", "healthz")]
    [InlineData("/__/gtg", "__/gtg", "healthz")]
    public async Task CustomHealthChecksTest(string? value, string url, params string[] errorUrls)
    {
        await using var factory = new WebApplicationFactory<Program>();

        using var client = factory
            .WithWebHostBuilder(builder =>
            {
                builder.SetupTestingEnvironment();
                if (value != null)
                {
                    builder.UseSetting("HealthCheck:Path", value);
                }
            })
            .CreateClientNoAutoRedirect();

        var content = await client.GetStringAsync(url);

        Assert.Equal("Healthy", content);

        foreach (var errorUrl in errorUrls)
        {
            await TestHelper.AssertIndexHtmlNotFoundAsync(client, errorUrl);
        }
    }
}
