namespace NetLah.Extensions.SpaServices.Hosting.Test;

public class AppFileVersionParserTest
{
    [Fact]
    public void ParseFolderSuccess()
    {
        var folderName = Path.Combine(TestHelper.ContentRoot, "public1");
        var appFileVersionInfo = new AppFileVersionParser().ParseFolder(folderName);
        TestHelper.AssertPublic1_Version(appFileVersionInfo);
    }

    [Fact]
    public void ParseFileSuccess()
    {
        var fileName = Path.Combine(TestHelper.ContentRoot, "public1", ".version");
        var appFileVersionInfo = new AppFileVersionParser().ParseFile(fileName);
        TestHelper.AssertPublic1_Version(appFileVersionInfo);
    }

    [Fact]
    public void ParseStreamSuccess()
    {
        var stream = TestHelper.GetEmbeddedResourceStream("public1..version");

#pragma warning disable CS8604 // Possible null reference argument.
        var appFileVersionInfo = new AppFileVersionParser().ParseStream(stream);
#pragma warning restore CS8604 // Possible null reference argument.

        TestHelper.AssertPublic1_Version(appFileVersionInfo);
    }

    [Fact]
    public void ParseSuccess2()
    {
        var fileContent = TestHelper.GetEmbeddedTemplateContent("public1..version");

#pragma warning disable CS8604 // Possible null reference argument.
        var appFileVersionInfo = new AppFileVersionParser().Parse(fileContent);
#pragma warning restore CS8604 // Possible null reference argument.

        TestHelper.AssertPublic1_Version(appFileVersionInfo);
    }

    [Fact]
    public void ParseReaderSuccess()
    {
        using var reader = new StringReader(@"app: The SPA
version: 1.2.3-dev7-1999
buildTime: 2022-09-18T09:58:19.7545003Z
description: This is free text");

        var appFileVersionInfo = new AppFileVersionParser().Parse(reader);
        Assert.NotNull(appFileVersionInfo);
        Assert.Equal("The SPA", appFileVersionInfo.Title);
        Assert.Equal("1.2.3-dev7-1999", appFileVersionInfo.Version);
        Assert.Equal("2022-09-18T09:58:19.7545003Z", appFileVersionInfo.BuildTimeString);
        Assert.Equal("This is free text", appFileVersionInfo.Description);
        Assert.Equal(new DateTimeOffset(2022, 9, 18, 9, 58, 19, TimeSpan.Zero).Add(TimeSpan.FromMilliseconds(754.5003D)), appFileVersionInfo.BuildTime);
    }

    [Fact]
    public void ParseSuccess()
    {
        var appFileVersionInfo = new AppFileVersionParser().Parse(@"app: The SPA
version: 1.2.3-dev7-1999
buildTime: 2022-09-18T09:58:19.7545286Z
description: This is free text");
        Assert.NotNull(appFileVersionInfo);
        Assert.Equal("The SPA", appFileVersionInfo.Title);
        Assert.Equal("1.2.3-dev7-1999", appFileVersionInfo.Version);
        Assert.Equal("2022-09-18T09:58:19.7545286Z", appFileVersionInfo.BuildTimeString);
        Assert.Equal("This is free text", appFileVersionInfo.Description);
        Assert.Equal(new DateTimeOffset(2022, 9, 18, 9, 58, 19, TimeSpan.Zero).Add(TimeSpan.FromMilliseconds(754.5286D)), appFileVersionInfo.BuildTime);
    }

    [Theory]
    [InlineData("public2Precedence", "The SPA .version.yml")]
    [InlineData("public3Precedence", "The SPA .version.yml")]
    [InlineData("public4Precedence", "The SPA .version.yml")]
    [InlineData("public5Precedence", "The SPA .version.yaml")]
    [InlineData("public6PrecedenceInvalidYml", "The SPA .version.yaml")]
    [InlineData("public7PrecedenceInvalidYmlYaml", "The SPA .version")]
    public void ParseFolderPrecedenceSuccess(string name, string expectation)
    {
        var folderName = Path.Combine(TestHelper.ContentRoot, name);
        var appFileVersionInfo = new AppFileVersionParser().ParseFolder(folderName);
        AssertPrecedence(appFileVersionInfo, expectation);
    }

    private static void AssertPrecedence(AppFileVersionInfo? appFileVersionInfo, string expectation)
    {
        Assert.NotNull(appFileVersionInfo);
        Assert.Equal(expectation, appFileVersionInfo.Title);
        Assert.Equal("1.2.3-dev7-1999", appFileVersionInfo.Version);
        Assert.Null(appFileVersionInfo.BuildTimeString);
        Assert.Null(appFileVersionInfo.Description);
        Assert.Null(appFileVersionInfo.BuildTime);
    }

    [Theory]
    [InlineData("public8Empty", true)]
    [InlineData("public9NotExist", false)]
    public void ParseFolderEmptyNotExistsSuccess(string name, bool folderExists)
    {
        var folderName = Path.Combine(TestHelper.ContentRoot, name);
        Assert.Equal(folderExists, Directory.Exists(folderName));
        var appFileVersionInfo = new AppFileVersionParser().ParseFolder(folderName);
        Assert.Null(appFileVersionInfo);
    }
}
