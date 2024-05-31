namespace WebApp.Test;

public class PagesTestingTest(DefaultWebApplicationFactory<Program> factory) : BasePagesTest<Program>(factory), IClassFixture<DefaultWebApplicationFactory<Program>>
{
    [Theory]
    [InlineData("/")]
    [InlineData("/any")]
    [InlineData("/any/")]
    [InlineData("/any/sub1")]
    [InlineData("/any/sub1/")]
    [InlineData("/any/sub1?q=value")]
    public Task IndexPageNotFound(string url) => AssertIndexHtmlNotFoundAsync(url);

    [Theory]
    [InlineData("/assets/site.min.css")]
    [InlineData("/assets/site.min.css?a=b")]
    public Task SiteCss(string url) => AssertGetStringAsync("/* site.min.css in wwwroot */", url);

    [Theory]
    [InlineData("/assets/site.release.js")]
    [InlineData("/assets/site.release.js?a=b")]
    public Task SiteJs(string url) => AssertGetStringAsync("/* site.release.js in wwwroot */", url);
}
