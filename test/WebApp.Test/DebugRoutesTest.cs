using Microsoft.AspNetCore.Mvc.Testing;

namespace WebApp.Test;

public class DebugRoutesTest
{
    [Theory]
    [InlineData(null, null, "debug/routes", "debug/routes1")]
    [InlineData("debug/routes", "debug/routes", "debug/routes1")]
    [InlineData("debug/routes", "debug/routes/", "debug/routes1")]
    public async Task DebugRoutesSuccess(string? value, string? url, params string[] errorUrls)
    {
        await using var factory = new WebApplicationFactory<Program>();

        using var client = factory
            .WithWebHostBuilder(builder =>
            {
                builder.SetupTestingEnvironment();
                if (!string.IsNullOrEmpty(value))
                {
                    builder.UseSetting("debugRoutes", value);
                }
            })
            .CreateClientNoAutoRedirect();

        if (!string.IsNullOrEmpty(url))
        {
            var content = await client.GetStringAsync(url);
            Assert.Equal(@"[] GET _general/Version NetLah.Extensions.SpaServices.Hosting.Controllers.GeneralController.Version (NetLah.Extensions.SpaServices.Hosting)
[] GET _general/Info NetLah.Extensions.SpaServices.Hosting.Controllers.GeneralController.Info (NetLah.Extensions.SpaServices.Hosting)
[] GET _general/Sys NetLah.Extensions.SpaServices.Hosting.Controllers.GeneralController.Sys (NetLah.Extensions.SpaServices.Hosting)
[] GET _general/Version Route: _general/Version
[] GET _general/Info Route: _general/Info
[] GET _general/Sys Route: _general/Sys
[] GET debug/routes HTTP: GET debug/routes
", content);
        }

        foreach (var errorUrl in errorUrls)
        {
            await TestHelper.AssertIndexHtmlNotFoundAsync(client, errorUrl);
        }
    }
}
