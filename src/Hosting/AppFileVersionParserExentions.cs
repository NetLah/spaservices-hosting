namespace NetLah.Extensions.SpaServices.Hosting;

internal static class AppFileVersionParserExentions
{
    public static AppFileVersionInfo? ParseFolder(this AppFileVersionParser parser, string folderName)
    {
        ArgumentNullException.ThrowIfNull(folderName);

        if (string.IsNullOrWhiteSpace(folderName))
        {
            throw new ArgumentException("folderName is required", nameof(folderName));
        }

        var result = ParseFile(parser, Path.Combine(folderName, ".version.yml"));
        result ??= ParseFile(parser, Path.Combine(folderName, ".version.yaml"));
        result ??= ParseFile(parser, Path.Combine(folderName, ".version"));
        return result;
    }

    public static AppFileVersionInfo? ParseFile(this AppFileVersionParser parser, string fileName)
    {
        ArgumentNullException.ThrowIfNull(fileName);

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("fileName is required", nameof(fileName));
        }

        var file = new FileInfo(fileName);
        if (!file.Exists)
        {
            return null;
        }

        using var reader = file.OpenText();
        return parser.Parse(reader);
    }

    public static AppFileVersionInfo? ParseStream(this AppFileVersionParser parser, Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using var reader = new StreamReader(stream);
        return parser.Parse(reader);
    }

    public static AppFileVersionInfo? Parse(this AppFileVersionParser parser, string? fileContent)
    {
        if (string.IsNullOrWhiteSpace(fileContent))
        {
            return default;
        }

        using var reader = new StringReader(fileContent);
        return parser.Parse(reader);
    }
}
