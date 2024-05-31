namespace WebApp.Test;

public class PagesPublic2Test(Public2CustomWebApplicationFactory factory) : BasePagesTest<Program>(factory), IClassFixture<Public2CustomWebApplicationFactory>
{
    [Theory]
    [InlineData("/")]
    [InlineData("/any")]
    [InlineData("/any/")]
    [InlineData("/any/sub1")]
    [InlineData("/any/sub1/")]
    [InlineData("/any/sub1?q=value")]
    [InlineData("/assets/site.release.js")]
    [InlineData("/assets/site.release.js?a=b")]
    public Task IndexPageNotFound(string url) => AssertIndexHtmlNotFoundAsync(url);

    [Theory]
    [InlineData("/assets/site.min.css")]
    [InlineData("/assets/site.min.css?a=b")]
    public Task SiteCss(string url) => AssertGetStringAsync("/* site.min.css in public2 */", url);
}
