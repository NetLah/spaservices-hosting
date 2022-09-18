using System.Text;

namespace NetLah.Extensions.SpaServices.Hosting.Test;

internal static class TestHelper
{
    public static readonly string ContentRoot = AppDomain.CurrentDomain.BaseDirectory;

    public static Stream? GetEmbeddedResourceStream(string fileName, string @namespace = "NetLah.Extensions.SpaServices.Hosting.Test")
    {
        var assembly = typeof(TestHelper).Assembly;
        var stream = assembly.GetManifestResourceStream($"{@namespace}.{fileName}");
        return stream;
    }

    public static string? GetEmbeddedTemplateContent(string fileName, string @namespace = "NetLah.Extensions.SpaServices.Hosting.Test")
    {
        var resourceStream = GetEmbeddedResourceStream(fileName, @namespace);
        if (resourceStream != null)
        {
            try
            {
                using var sr = new StreamReader(resourceStream, Encoding.UTF8, true);
                return sr.ReadToEnd();
            }
            finally
            {
                resourceStream.Dispose();
            }
        }
        return default;
    }

    public static void AssertPublic1_Version(AppFileVersionInfo? appFileVersionInfo)
    {
        Assert.NotNull(appFileVersionInfo);
        Assert.Equal("The SPA .version", appFileVersionInfo.Title);
        Assert.Equal("1.2.3-dev7-1999", appFileVersionInfo.Version);
        Assert.Equal("2022-09-18T09:58:19.7545001Z", appFileVersionInfo.BuildTimeString);
        Assert.Equal("This is free text", appFileVersionInfo.Description);
        Assert.Equal(new DateTimeOffset(2022, 9, 18, 9, 58, 19, TimeSpan.Zero).Add(TimeSpan.FromMilliseconds(754.5001D)), appFileVersionInfo.BuildTime);
    }
}
