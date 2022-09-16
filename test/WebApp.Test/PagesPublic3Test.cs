namespace WebApp.Test;

public class PagesPublic3Test : BasePagesTest<Program>, IClassFixture<Public3CustomWebApplicationFactory>
{
    public PagesPublic3Test(Public3CustomWebApplicationFactory factory) : base(factory) { }

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
    public Task SiteCss(string url) => AssertGetStringAsync("/* site.min.css in public3 */", url);
}
