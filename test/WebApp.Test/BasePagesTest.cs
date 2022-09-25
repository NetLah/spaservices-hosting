using Microsoft.AspNetCore.Mvc.Testing;

namespace WebApp.Test;

public abstract class BasePagesTest<TEntryPoint> where TEntryPoint : class
{
    protected readonly HttpClient _client;

    protected BasePagesTest(WebApplicationFactory<TEntryPoint> factory)
        => _client = factory.CreateClientNoAutoRedirect();

    protected async Task AssertIndexHtmlNotFoundAsync(string url)
    {
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _client.GetAsync(url));
        Assert.Equal("The SPA default page middleware could not return the default page '/index.html' because it was not found, and no other middleware handled the request.\n", ex.Message);
    }

    protected async Task AssertGetStringAsync(string expected, string url)
    {
        var content = await _client.GetStringAsync(url);
        Assert.Equal(expected, content);
    }

    [Fact]
    public virtual async Task GeneralInfoUrl()
    {
        var content = await _client.GetStringAsync("/_general/info");
        Assert.StartsWith("App:testhost; Version:", content);
    }
}
