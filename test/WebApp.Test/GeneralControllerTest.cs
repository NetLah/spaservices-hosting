using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NetLah.Extensions.SpaServices.Hosting;

namespace WebApp.Test;

public class GeneralControllerTest
{
    [Fact]
    public async Task GeneralVersionUrl()
    {
        var appInfoMock = new Mock<IAppInfo>();
        appInfoMock.SetupGet(m => m.Version).Returns("v1-alpha1").Verifiable();

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

        appInfoMock.Invocations.Clear();    // reset invocation by application lifetime

        var content = await client.GetStringAsync("_general/version");

        Assert.Equal("v1-alpha1", content);

        appInfoMock.VerifyAll();
        appInfoMock.VerifyGet(m => m.Title, Times.Never);
        appInfoMock.VerifyGet(m => m.BuildTimestampLocal, Times.Never);
    }

    [Fact]
    public async Task GeneralInfoUrl()
    {
        var appInfoMock = new Mock<IAppInfo>();
        appInfoMock.SetupGet(m => m.Title).Returns("title1").Verifiable();
        appInfoMock.SetupGet(m => m.Version).Returns("v1-alpha2").Verifiable();
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

        appInfoMock.VerifyAll();
        appInfoMock.VerifyGet(m => m.Title, Times.Once);
        appInfoMock.VerifyGet(m => m.Version, Times.Once);
        appInfoMock.VerifyGet(m => m.BuildTimestampLocal, Times.Once);
        appInfoMock.VerifyGet(m => m.HostBuildTimestampLocal, Times.Once);  // application lifetime but not initializing

        appInfoMock.Invocations.Clear();

        var content = await client.GetStringAsync("_general/info");

        Assert.Equal("App:title1; Version:v1-alpha2; BuildTime:buildTimestamp1 Scheme:http Host:localhost :0", content);

        appInfoMock.VerifyAll();
        appInfoMock.VerifyGet(m => m.Title, Times.Once);
        appInfoMock.VerifyGet(m => m.Version, Times.Once);
        appInfoMock.VerifyGet(m => m.BuildTimestampLocal, Times.Once);
        appInfoMock.VerifyGet(m => m.HostBuildTimestampLocal, Times.Never);
    }

    [Theory]
    [InlineData("RouteGeneralInfo", "", "_general/info")]
    [InlineData("RouteGeneralInfo", "/debug/general/info", "/debug/general/info")]
    [InlineData("RouteGeneralInfo", "debug/general/info", "/debug/general/info")]
    [InlineData("RouteGeneralInfo", "debug/general/info", "/debug/general/info?a=b")]
    [InlineData("RouteGeneralInfo", "debug/general/info", "/debug/general/info/?a=b")]
    [InlineData("RouteGeneral", "debug/general1/{action}", "/debug/general1/info")]
    [InlineData("RouteGeneral", "debug/general1/{action}", "/debug/general1/info/")]
    [InlineData("RouteGeneral", "debug/general1/{action}", "/debug/general1/info?someQuery")]
    [InlineData("RouteGeneral", "debug/general1/{action}", "/debug/general1/info/?someQuery")]
    [InlineData("RouteGeneral", "debug/general/2/{action}", "/debug/general/2/info")]
    [InlineData("RouteGeneral", "debug/general/Get{action}", "/debug/general/GetInfo")]
    public async Task CustomRouteActionGeneralInfoUrl(string key, string value, string url)
    {
        await using var factory = new WebApplicationFactory<Program>();

        using var client = factory
            .WithWebHostBuilder(builder =>
            {
                builder.SetupTestingEnvironment();
                builder.UseSetting(key, value);
            })
            .CreateClientNoAutoRedirect();

        var content = await client.GetStringAsync(url);

        Assert.StartsWith("App:testhost; Version:", content);
    }

    [Theory]
    [InlineData("RouteGeneralVersion", "", "_general/version")]
    [InlineData("RouteGeneralVersion", "/debug/general/ver", "/debug/general/ver")]
    [InlineData("RouteGeneralVersion", "debug/general/ver", "/debug/general/ver")]
    [InlineData("RouteGeneralVersion", "debug/general/ver", "/debug/general/ver?a=b")]
    [InlineData("RouteGeneralVersion", "debug/general/ver", "/debug/general/ver/?a=b")]
    [InlineData("RouteGeneral", "debug/general1/{action}", "/debug/general1/version")]
    [InlineData("RouteGeneral", "debug/general1/{action}", "/debug/general1/version/")]
    [InlineData("RouteGeneral", "debug/general1/{action}", "/debug/general1/version?someQuery")]
    [InlineData("RouteGeneral", "debug/general1/{action}", "/debug/general1/version/?someQuery")]
    [InlineData("RouteGeneral", "debug/general/2/{action}", "/debug/general/2/version")]
    [InlineData("RouteGeneral", "debug/general/Get{action}", "/debug/general/GetVersion")]
    public async Task CustomRouteActionGeneralVersionUrl(string key, string value, string url)
    {
        var appInfoMock = new Mock<IAppInfo>();
        appInfoMock.SetupGet(m => m.Version).Returns("v1-alpha3").Verifiable();

        await using var factory = new WebApplicationFactory<Program>();

        using var client = factory
            .WithWebHostBuilder(builder =>
            {
                builder.SetupTestingEnvironment();

                builder.ConfigureServices(services =>
                {
                    services.AddSingleton<IAppInfo>(_ => appInfoMock.Object);
                });

                builder.UseSetting(key, value);
            })
            .CreateClientNoAutoRedirect();

        appInfoMock.Invocations.Clear();    // reset invocation by application lifetime

        var content = await client.GetStringAsync(url);

        Assert.Equal("v1-alpha3", content);

        appInfoMock.VerifyAll();
    }
}
