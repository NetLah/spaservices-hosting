using Microsoft.AspNetCore.Mvc.Testing;

namespace WebApp.Test;

public class HealthChecksTest
{
    [Theory]
    [InlineData(null, "_healthz")]
    [InlineData("", "_healthz")]
    [InlineData("/_healthz", "_healthz")]
    [InlineData("/__gtg", "__gtg", "_healthz")]
    [InlineData("/__/gtg", "__/gtg", "_healthz")]
    public async Task CustomRouteTest(string? value, string url, params string[] errorUrls)
    {
        await using var factory = new WebApplicationFactory<Program>();

        using var client = factory
            .WithWebHostBuilder(builder =>
            {
                builder.SetupTestingEnvironment();
                if (value != null)
                {
                    builder.UseSetting("healthChecksPath", value);
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
