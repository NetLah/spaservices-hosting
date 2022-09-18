using System.Globalization;

namespace NetLah.Extensions.SpaServices.Hosting;

internal static class BuildTimeHelper
{
    public static DateTimeOffset? ParseBuildTime(string? value)
        => !string.IsNullOrEmpty(value) &&
            DateTimeOffset.TryParseExact(value,
                new string[] {
                "yyyy-MM-ddTHH:mm:ss.fffffff", "yyyy-MM-ddTHH:mm:ss.fffffffZ",
                "yyyy-MM-ddTHH:mm:ss.fff", "yyyy-MM-ddTHH:mm:ss.fffZ",

                "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:ssZ",

                "yyyy-MM-ddTHH:mm:ss.f", "yyyy-MM-ddTHH:mm:ss.fZ",
                "yyyy-MM-ddTHH:mm:ss.ff", "yyyy-MM-ddTHH:mm:ss.ffZ",
                "yyyy-MM-ddTHH:mm:ss.ffff", "yyyy-MM-ddTHH:mm:ss.ffffZ",
                "yyyy-MM-ddTHH:mm:ss.fffff", "yyyy-MM-ddTHH:mm:ss.fffffZ",
                "yyyy-MM-ddTHH:mm:ss.ffffff", "yyyy-MM-ddTHH:mm:ss.ffffffZ"
                },
                CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var result) ?
            result :
            null;
}
