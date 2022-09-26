using Microsoft.AspNetCore.Mvc.Testing;

namespace WebApp.Test;

public abstract class BasePagesTest<TEntryPoint> where TEntryPoint : class
{
    protected readonly HttpClient _client;

    protected BasePagesTest(WebApplicationFactory<TEntryPoint> factory)
        => _client = factory.CreateClientNoAutoRedirect();

    protected Task AssertIndexHtmlNotFoundAsync(string url) => TestHelper.AssertIndexHtmlNotFound(_client, url);

    protected async Task AssertGetStringAsync(string expected, string url)
    {
        var content = await _client.GetStringAsync(url);
        Assert.Equal(expected, content);
    }

    [Fact]
    public virtual async Task GeneralInfoUrl()
    {
        var content = await _client.GetStringAsync("_general/info");
        Assert.StartsWith("App:testhost; Version:", content);
    }
}
