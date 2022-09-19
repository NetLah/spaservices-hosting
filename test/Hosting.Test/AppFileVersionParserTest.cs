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
    public void ParseFileContentSuccess()
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

    [Theory]
    [InlineData(false, null, null, null, null, null)]
    [InlineData(false, null, null, null, null, "")]
    [InlineData(false, null, null, null, null, " \r \r ")]
    [InlineData(false, null, null, null, null, "app: \r\rversion: ")]
    [InlineData(false, null, null, null, null, "app: The App")]
    [InlineData(false, null, null, null, null, "app: The App\r\rversion:\r")]
    [InlineData(true, null, "v1", null, null, "v1")]
    [InlineData(true, null, "v2", null, null, "v2\r \r")]
    [InlineData(true, null, "v3", null, null, "app:\rversion:v3\rbuildTime:\rdescription:")]
    [InlineData(true, null, "v4", null, null, "version:v4\rbuildTime:2022-09-18T09:58")]
    [InlineData(true, "title5", "v5", null, null, "app:  title5  \rversion: v5   \r")]
    [InlineData(true, null, "v6", "2022-09-18T09:58:19", null, "BuildTime: 2022-09-18T09:58:19\rVersion:v6")]
    [InlineData(true, null, "v7", null, "The description 7", "version:v7\rdescription: The description 7")]
    [InlineData(true, "title8", "v8", "2022-09-18T09:58:19.1234567Z", "The desc 8", "  app  : title8 \r  version  : v8 \r  buildTime  : 2022-09-18T09:58:19.1234567Z \r  description  : The desc 8 ")]
    [InlineData(true, "title9", "v9", "2022-09-18T09:58:19.1234567Z", "The description 9", "app: title9 \n  version: v9 \n\rbuildTime: 2022-09-18T09:58:19.1234567Z \r\ndescription: The description 9\r\n")]
    [InlineData(true, null, "v10", null, null, "version:v10\rversion:v1\rversion:v2")]
    public void Parse(bool n, string a, string v, string t, string d, string content)
    {
        var appFileVersionInfo = new AppFileVersionParser().Parse(content);
        if (n)
        {
            Assert.NotNull(appFileVersionInfo);
            Assert.Equal(a, appFileVersionInfo.Title);
            Assert.Equal(v, appFileVersionInfo.Version);
            Assert.Equal(t, appFileVersionInfo.BuildTimeString);
            Assert.Equal(d, appFileVersionInfo.Description);
        }
        else
        {
            Assert.Null(appFileVersionInfo);
        }
    }
}
