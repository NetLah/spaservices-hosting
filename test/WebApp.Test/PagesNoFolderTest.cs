namespace WebApp.Test;

public class PagesNoFolderTest(NoFolderCustomWebApplicationFactory factory) : BasePagesTest<Program>(factory), IClassFixture<NoFolderCustomWebApplicationFactory>
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
    [InlineData("/assets/site.min.css")]
    [InlineData("/assets/site.min.css?a=b")]
    public Task IndexPageNotFound(string url) => AssertIndexHtmlNotFoundAsync(url);

}
