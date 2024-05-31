using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using NetLah.Extensions.Configuration;

namespace NetLah.Extensions.SpaServices.Hosting.Test;

public class MountFileHelpersTest
{
    private static Func<string, string> GetFullPathDelegate = null!;

    public MountFileHelpersTest()
    {
        MountFileHelpers.ValidateDirectoryExistsDelegate = _ => true;
        GetFullPathDelegate = OperatingSystem.IsWindows() ? Path.GetFullPath : _ => _;
    }

    private static MountFileProviderOptions ParserOptions(IConfigurationRoot configuration)
        => MountFileHelpers.ParserOptions(configuration, NullLogger.Instance);

    private static string[] CovertAbsolutePath(string[] array)
    {
        return OperatingSystem.IsWindows() ? array.Select(Path.GetFullPath).ToArray() : array;
    }

    [Fact]
    public void SourceFilesTest()
    {
        var configuration = ConfigurationBuilderBuilder.Create().WithInMemory(new Dictionary<string, string?>
        {
            ["MountFile:0"] = "/config/app.json=/test/config/app.json",
            ["MountFile:1:target"] = "/config/app1.json",
            ["MountFile:1:from"] = "/test/config/app1.json",
            ["mountFile:2:target"] = "/config/app2.json",
            ["mountFile:2:source"] = "/test/config/app2.json",
            ["mountFile:3:target"] = "/config/app3.json",
            ["mountFile:3:src"] = "/test/config/app3.json",
        }).Manager;

        var options = ParserOptions(configuration);

        Assert.NotNull(options);
        Assert.Empty(options.Folders);
        Assert.Equal(CovertAbsolutePath([
            "/test/config/app.json",
            "/test/config/app1.json",
            "/test/config/app2.json",
            "/test/config/app3.json",
        ]), options.Files.Values);
    }

    [Fact]
    public void TargetFilesTest()
    {
        var configuration = ConfigurationBuilderBuilder.Create().WithInMemory(new Dictionary<string, string?>
        {
            ["mountFile:0"] = "/config/app.json=/test/config/app.json",
            ["mountFile:1:target"] = "/config/app1.json",
            ["mountFile:1:source"] = "/test/config/app1.json",
            ["mountFile:2:to"] = "/config/app2.json",
            ["mountFile:2:source"] = "/test/config/app2.json",
            ["MountFile:3:destination"] = "/config/app3.json",
            ["MountFile:3:source"] = "/test/config/app3.json",
            ["MountFile:4:dest"] = "/config/app4.json",
            ["MountFile:4:source"] = "/test/config/app4.json",
            ["MountFile:5:dst"] = "/config/app5.json",
            ["MountFile:5:source"] = "/test/config/app5.json",
        }).Manager;

        var options = ParserOptions(configuration);

        Assert.NotNull(options);
        Assert.Empty(options.Folders);
        Assert.Equal([
            "/config/app.json",
            "/config/app1.json",
            "/config/app2.json",
            "/config/app3.json",
            "/config/app4.json",
            "/config/app5.json",
        ], options.Files.Keys);
    }

    [Fact]
    public void MountFileOrMountFilesTest()
    {
        var configuration = ConfigurationBuilderBuilder.Create().WithInMemory(new Dictionary<string, string?>
        {
            ["MountFile:0"] = "/config/app.json=/test/config/app.json",
            ["MountFiles:1:target"] = "/config/app1.json",
            ["MountFiles:1:source"] = "/test/config/app1.json",
        }).Manager;

        var options = ParserOptions(configuration);

        Assert.NotNull(options);
        Assert.Empty(options.Folders);

        Assert.Equal(new Dictionary<string, string>
        {
            ["/config/app.json"] = GetFullPathDelegate("/test/config/app.json"),
            ["/config/app1.json"] = GetFullPathDelegate("/test/config/app1.json"),
        }, options.Files);
    }

    [Fact]
    public void SourceFolderTest()
    {
        var configuration = ConfigurationBuilderBuilder.Create().WithInMemory(new Dictionary<string, string?>
        {
            ["MountFolder:0"] = "/config=/test/config",
            ["MountFolder:1:target"] = "/config1",
            ["MountFolder:1:from"] = "/test/config1",
            ["mountFolder:2:target"] = "/config2",
            ["mountFolder:2:source"] = "/test/config2/",
            ["mountFolder:3:target"] = "/config3",
            ["mountFolder:3:src"] = "/test/config3/",
        }).Manager;

        var options = ParserOptions(configuration);

        Assert.NotNull(options);
        Assert.Empty(options.Files);
        Assert.Equal(CovertAbsolutePath([
            "/test/config",
            "/test/config1",
            "/test/config2/",
            "/test/config3/",
        ]), options.Folders.Values);
    }

    [Fact]
    public void TargetFolderTest()
    {
        var configuration = ConfigurationBuilderBuilder.Create().WithInMemory(new Dictionary<string, string?>
        {
            ["mountFolder:0"] = "/config=/test/config",
            ["mountFolder:1:target"] = "/config1",
            ["mountFolder:1:source"] = "/test/config1",
            ["mountFolder:2:to"] = "/config2",
            ["mountFolder:2:source"] = "/test/config2/",
            ["MountFolder:3:destination"] = "/config3/",
            ["MountFolder:3:source"] = "/test/config3",
            ["MountFolder:4:dest"] = "/config4/",
            ["MountFolder:4:source"] = "/test/config4",
            ["MountFolder:5:dst"] = "/config5/",
            ["MountFolder:5:source"] = "/test/config5",
        }).Manager;

        var options = ParserOptions(configuration);

        Assert.NotNull(options);
        Assert.Empty(options.Files);
        Assert.Equal([
            "/config/",
            "/config1/",
            "/config2/",
            "/config3/",
            "/config4/",
            "/config5/",
        ], options.Folders.Keys);
    }

    [Fact]
    public void MountFolderOrMountFoldersTest()
    {
        var configuration = ConfigurationBuilderBuilder.Create().WithInMemory(new Dictionary<string, string?>
        {
            ["MountFolder:0"] = "/config=/test/config",
            ["MountFolders:1:target"] = "/config1/",
            ["MountFolders:1:source"] = "/test/config1/",
        }).Manager;

        var options = ParserOptions(configuration);

        Assert.NotNull(options);
        Assert.Empty(options.Files);

        Assert.Equal(new Dictionary<string, string>
        {
            ["/config/"] = GetFullPathDelegate("/test/config"),
            ["/config1/"] = GetFullPathDelegate("/test/config1/"),
        }, options.Folders);
    }

    [Theory]
    [InlineData("config/app.json", "Invalid MountFile 'config/app.json'")]
    [InlineData("config/app.json=", "Invalid MountFile 'config/app.json='")]
    [InlineData("=/test/config", "Invalid MountFile '=/test/config'")]
    [InlineData("config/app.json=/test/config", "target has to start with / '")]
    public void InvalidMountFileTest(string configValue, string error)
    {
        var configuration = ConfigurationBuilderBuilder.Create().WithInMemory(new Dictionary<string, string?>
        {
            ["MountFile:0"] = configValue,
        }).Manager;

        var ex = Assert.Throws<InvalidOperationException>(() => ParserOptions(configuration));
        Assert.StartsWith(error, ex.Message);
    }

    [Fact]
    public void MountFileDuplicatedTest()
    {
        var configuration = ConfigurationBuilderBuilder.Create().WithInMemory(new Dictionary<string, string?>
        {
            ["MountFile:0"] = "/config/app.json=/test/config/app.json",
            ["MountFiles:1"] = "/config/app.json=/test/config/app.json",
        }).Manager;

        var ex = Assert.Throws<InvalidOperationException>(() => ParserOptions(configuration));
        Assert.Equal("Duplicated target '/config/app.json'", ex.Message);
    }

    [Theory]
    [InlineData("config/", "Invalid MountFolder 'config/'")]
    [InlineData("config=", "Invalid MountFolder 'config='")]
    [InlineData("=/test/config", "Invalid MountFolder '=/test/config'")]
    [InlineData("config=/test/config", "target has to start with / '")]
    public void InvalidMountFolderTest(string configValue, string error)
    {
        var configuration = ConfigurationBuilderBuilder.Create().WithInMemory(new Dictionary<string, string?>
        {
            ["MountFolder:0"] = configValue,
        }).Manager;

        var ex = Assert.Throws<InvalidOperationException>(() => ParserOptions(configuration));
        Assert.StartsWith(error, ex.Message);
    }

    [Fact]
    public void MountFolderDuplicatedTest()
    {
        var configuration = ConfigurationBuilderBuilder.Create().WithInMemory(new Dictionary<string, string?>
        {
            ["MountFolder:0"] = "/config/=/test/config/",
            ["MountFolders:1"] = "/config=/test/config",
        }).Manager;

        var ex = Assert.Throws<InvalidOperationException>(() => ParserOptions(configuration));
        Assert.Equal("Duplicated target '/config/'", ex.Message);
    }
}
