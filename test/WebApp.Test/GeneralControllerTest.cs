using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NetLah.Extensions.SpaServices.Hosting;

namespace WebApp.Test;

public class GeneralControllerTest
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

    [Fact]
    public async Task GetInfoUrl()
    {
        var appInfoMock = new Mock<IAppInfo>();
        appInfoMock.SetupGet(m => m.Title).Returns("title1").Verifiable();
        appInfoMock.SetupGet(m => m.Version).Returns("v1-alpha1").Verifiable();
        appInfoMock.SetupGet(m => m.BuildTimestampLocal).Returns("buildTimestamp1").Verifiable();

        await using var factory = new WebApplicationFactory<Program>();

        using var client = factory
            .WithWebHostBuilder(builder =>
            {
                builder.SetupTestingEnvironment();

                builder.ConfigureServices(services =>
                {
                    services.AddSingleton<IAppInfo>(_ => appInfoMock.Object);
                });
            })
            .CreateClientNoAutoRedirect();

        var content = await client.GetStringAsync("/api/v1/general/getInfo");

        Assert.StartsWith("App:title1; Version:v1-alpha1; BuildTime:buildTimestamp1", content);

        appInfoMock.VerifyAll();
        appInfoMock.VerifyGet(m => m.Title, Times.Exactly(2));  // application lifetime but not initializing
        appInfoMock.VerifyGet(m => m.HostBuildTimestampLocal, Times.Once);
    }
}
