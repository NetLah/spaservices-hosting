using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace WebApp.Test;

public class ForwardedHeadersTest
{
    [Theory]
    [InlineData(false, "http://localhost1/", "http")]
    [InlineData(true, "http://localhost2/", "https")]
    public async Task ForwardedHeadersEnabledTest(bool forwardedHeadersEnabled, string url, string scheme)
    {
        var builder = TestHelper.CreateWebApplicationBuilder();
        builder.Configuration["FORWARDEDHEADERS_ENABLED"] = forwardedHeadersEnabled.ToString().ToLower();
        await using var app = builder.Build();

        app.Run(context =>
        {
            Assert.Equal(scheme, context.Request.Scheme);
            return Task.CompletedTask;
        });

        await app.StartAsync();

        var client = app.GetTestClient();
        client.DefaultRequestHeaders.Add("x-forwarded-proto", "https");
        var result = await client.GetAsync(url);
        result.EnsureSuccessStatusCode();
    }
}
