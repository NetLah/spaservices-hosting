using Microsoft.Extensions.FileProviders;
using Moq;

namespace NetLah.Extensions.SpaServices.Hosting.Test;

public class MountFileProviderTest
{
    private static readonly StringComparer _stringComparer = OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

    [Fact]
    public void MountFileTest()
    {
        var service = new MountFileProvider(null!, new MountFileProviderOptions
        {
            Files = new Dictionary<string, string>(_stringComparer)
            {
                ["/config1/app2.json"] = "mount-file-folder/app.Prd.json",
            }
        });

        var result = service.GetFileInfo("/config1/app2.json");

        Assert.NotNull(result);
        Assert.True(result.Exists);
    }

    [Fact]
    public void MountFileNotFoundTest()
    {
        var service = new MountFileProvider(null!, new MountFileProviderOptions
        {
            Files = new Dictionary<string, string>(_stringComparer)
            {
                ["/config3/app4.json"] = "mount-file-folder/app.not-found.json",
            }
        });

        var result = service.GetFileInfo("/config3/app4.json");

        Assert.NotNull(result);
        Assert.False(result.Exists);
    }

    [Fact]
    public void MountFileFallbackTest()
    {
        var mockFileProvider = new Mock<IFileProvider>();
        var service = new MountFileProvider(mockFileProvider.Object, new MountFileProviderOptions
        {
            Files = new Dictionary<string, string>(_stringComparer)
            {
                ["/config/app.json"] = "mount-file-folder/any.json",
            }
        });

        _ = service.GetFileInfo("/config3/app4.json");

        mockFileProvider.Verify(x => x.GetFileInfo("/config3/app4.json"), Times.Once());
        mockFileProvider.Verify(x => x.GetFileInfo(It.IsAny<string>()), Times.Once());
    }

    [Fact]
    public void MountFolderTest()
    {
        var service = new MountFileProvider(null!, new MountFileProviderOptions
        {
            Folders = new Dictionary<string, string>(_stringComparer)
            {
                ["/config1/"] = Path.GetFullPath("mount-file-folder"),
            }
        });

        var result = service.GetFileInfo("/config1/app.Prd.json");

        Assert.NotNull(result);
        Assert.True(result.Exists);
        Assert.Equal("app.Prd.json", result.Name);
    }

    [Fact]
    public void MountFolderNotFoundTest()
    {
        var service = new MountFileProvider(null!, new MountFileProviderOptions
        {
            Folders = new Dictionary<string, string>(_stringComparer)
            {
                ["/config2/"] = Path.GetFullPath("mount-file-folder"),
            }
        });

        Assert.True(service.GetFileInfo("/config2/app.Prd.json").Exists);

        var result = service.GetFileInfo("/config2/app.not-exists.json");

        Assert.NotNull(result);
        Assert.False(result.Exists);
        Assert.Equal("app.not-exists.json", result.Name);
    }

    [Fact]
    public void MountFolderFallbackTest()
    {
        var mockFileProvider = new Mock<IFileProvider>();
        var service = new MountFileProvider(mockFileProvider.Object, new MountFileProviderOptions
        {
            Folders = new Dictionary<string, string>(_stringComparer)
            {
                ["/config3/"] = Path.GetFullPath("mount-file-folder"),
            }
        });

        Assert.True(service.GetFileInfo("/config3/app.Prd.json").Exists);

        _ = service.GetFileInfo("/config4/app4.json");

        mockFileProvider.Verify(x => x.GetFileInfo("/config4/app4.json"), Times.Once());
        mockFileProvider.Verify(x => x.GetFileInfo(It.IsAny<string>()), Times.Once());
    }
}
