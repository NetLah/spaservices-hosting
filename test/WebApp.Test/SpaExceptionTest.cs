﻿namespace WebApp.Test;

public class SpaExceptionTest
{
    [Theory]
    [InlineData("NoFolder", false)]
    [InlineData("Testing", false)]
    [InlineData("Public", true, "")]
    public async Task FolderNoIndexHtml(string environmentName, bool hasIndex, params string[] urls)
    {
        await using var factory = new DefaultWebApplicationFactory<Program>() { EnvironmentName = environmentName };

        using var client = factory.CreateClientNoAutoRedirect();

        if (hasIndex)
        {
            Assert.NotEmpty(urls);
            foreach (var url in urls)
            {
                var content = await client.GetStringAsync(url);
                Assert.NotEmpty(content);
            }
        }
        else
        {
            Assert.Empty(urls);
            await TestHelper.AssertIndexHtmlNotFoundAsync(client, "");
        }
    }

    [Theory]
    [InlineData("Public", "POST", "")]
    [InlineData("Public", "PUT", "")]
    [InlineData("Public", "DELETE", "")]
    [InlineData("Public", "PATCH", "")]
    [InlineData("Public", "OPTIONS", "")]
    public async Task SpaNotSupportHttpMethod(string environmentName, string method, string url)
    {
        var asserted = false;
        await using var factory = new DefaultWebApplicationFactory<Program>() { EnvironmentName = environmentName };

        using var client = factory.CreateClientNoAutoRedirect();

        switch (method)
        {
            case "POST": asserted = true; await TestHelper.AssertIndexHtmlNotFoundAsync(() => client.PostAsync(url, EmptyContent())); break;
            case "PUT": asserted = true; await TestHelper.AssertIndexHtmlNotFoundAsync(() => client.PutAsync(url, EmptyContent())); break;
            case "PATCH": asserted = true; await TestHelper.AssertIndexHtmlNotFoundAsync(() => client.PatchAsync(url, EmptyContent())); break;
            case "DELETE": asserted = true; await TestHelper.AssertIndexHtmlNotFoundAsync(() => client.DeleteAsync(url)); break;
            case "OPTIONS": asserted = true; await TestHelper.AssertIndexHtmlNotFoundAsync(() => client.SendAsync(new HttpRequestMessage(HttpMethod.Options, url))); break;
            default: Assert.Fail("Invalid method"); break;
        }

        Assert.True(asserted);

        static HttpContent? EmptyContent()
        {
            return default;
        }
    }
}
