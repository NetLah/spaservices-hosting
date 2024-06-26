﻿using Microsoft.AspNetCore.Mvc.Testing;

namespace WebApp.Test;

public abstract class BasePagesTest<TEntryPoint>(WebApplicationFactory<TEntryPoint> factory) where TEntryPoint : class
{
    protected readonly HttpClient _client = factory.CreateClientNoAutoRedirect();

    protected Task AssertIndexHtmlNotFoundAsync(string url) => TestHelper.AssertIndexHtmlNotFoundAsync(_client, url);

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
