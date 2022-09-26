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
    public async Task CustomRouteActionGeneralInfoUrl(string? value, string url, params string[] errorUrls)
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

        foreach (var item in errorUrls)
        {
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => client.GetAsync(item));
            Assert.Equal("The SPA default page middleware could not return the default page '/index.html' because it was not found, and no other middleware handled the request.\n", ex.Message);
        }
    }
}
