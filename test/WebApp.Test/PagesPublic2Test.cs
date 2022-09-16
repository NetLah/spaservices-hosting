namespace WebApp.Test;

public class PagesPublic2Test : BasePagesTest<Program>, IClassFixture<Public2CustomWebApplicationFactory>
{
    public PagesPublic2Test(Public2CustomWebApplicationFactory factory) : base(factory) { }

    [Theory]
    [InlineData("/")]
    [InlineData("/any")]
    [InlineData("/any/")]
    [InlineData("/any/sub1")]
    [InlineData("/any/sub1/")]
    [InlineData("/any/sub1?q=value")]
    public Task IndexPage(string url) => AssertGetStringAsync("<html><body><p>index.html in public2</p></body></html>", url);

    [Theory]
    [InlineData("/assets/site.min.css")]
    [InlineData("/assets/site.min.css?a=b")]
    public Task SiteCss(string url) => AssertGetStringAsync("/* site.min.css in public2 */", url);

    [Theory]
    [InlineData("/assets/site.release.js")]
    [InlineData("/assets/site.release.js?a=b")]
    public Task SiteJs(string url) => AssertGetStringAsync("/* site.release.js in public2 */", url);
}
