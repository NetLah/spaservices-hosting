namespace WebApp.Test;

public class PagesPublicTest : BasePagesTest<Program>, IClassFixture<PublicCustomWebApplicationFactory>
{
    public PagesPublicTest(PublicCustomWebApplicationFactory factory) : base(factory) { }

    [Theory]
    [InlineData("/")]
    [InlineData("/any")]
    [InlineData("/any/")]
    [InlineData("/any/sub1")]
    [InlineData("/any/sub1/")]
    [InlineData("/any/sub1?q=value")]
    public Task IndexPage(string url) => AssertGetStringAsync("<html><body><p>This is test index.html in public</p></body></html>", url);

    [Theory]
    [InlineData("/assets/site.min.css")]
    [InlineData("/assets/site.min.css?a=b")]
    public Task SiteCss(string url) => AssertGetStringAsync("/* site.min.css in public */", url);

    [Theory]
    [InlineData("/assets/site.release.js")]
    [InlineData("/assets/site.release.js?a=b")]
    public Task SiteJs(string url) => AssertGetStringAsync("/* site.release.js in public */", url);
}
